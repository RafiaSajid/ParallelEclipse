using UnityEngine;
using System.Collections;
using System;

//[RequireComponent(typeof(SpriteRenderer))] // ensures that any GameObject with this component also has a SpriteRenderer. 
// Unity will auto-add one if missing and 
// prevents GetComponent returning null at runtime. Useful for contracts.
public class LunarShadowMover : MonoBehaviour // moves  a shadow across bg mask and alpha changes when over moon
{
    [Header("Movement")]
    public Transform startPoint;          // place an empty GameObject for start
    public Transform endPoint;            // place an empty GameObject for end
    public float duration = 5f;           // how long it takes to move

    [Header("Alpha (Transparency)")]
    [Range(0f, 1f)] public float offAlpha = 0.25f; // transparency when NOT over the moon
    [Range(0f, 10f)] public float onAlpha = 10f;     // transparency when OVER the moon (fully opaque)
    public float alphaSmoothSpeed = 10f;   // higher = faster the alpha changes

    [Header("Moon Detection")]
    public SpriteRenderer background;     // Drag your BACKGROUND GameObject's SpriteRenderer here
    public Texture2D moonMask;            // Drag your Moon Mask Texture2D here (make sure Read/Write is enabled!)
    [Range(0f, 1f)] public float maskThreshold = 0.5f; // How "white" a pixel needs to be to count as moon

    // Internal variables
    private SpriteRenderer sr;
    private Vector3 fromPos, toPos;
    private float timer;
    private bool moving;

    public static event Action OnEclipseAnimationComplete;



    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("SimpleLunarShadow requires a SpriteRenderer component on this GameObject.");
            enabled = false; // Disable script if no SpriteRenderer
            //avoids null-reference errors at runtime.
            return;
        }

        // Basic validation for moon detection
        if (background == null) Debug.LogWarning("Background SpriteRenderer not assigned for SimpleLunarShadow on " + gameObject.name);

        if (moonMask == null) Debug.LogWarning("Moon Mask Texture2D not assigned for SimpleLunarShadow on " + gameObject.name);

        else if (!moonMask.isReadable) Debug.LogError("Moon Mask Texture2D for SimpleLunarShadow on " + gameObject.name + " is not readable. Please enable 'Read/Write Enabled' in its import settings.");
    }

    void Start()
    {
        // Set initial position
        if (startPoint != null)
        {
            transform.position = startPoint.position;
        }
        else
        {
            // If no startPoint, initialize 'fromPos' to current position
            fromPos = transform.position;
        }

        SetAlpha(offAlpha); // Start with off-moon transparency
        moving = false; // Not moving until PlayDelayed is called
    }

    void Update()
    {
        if (!moving) return;

        // 1. Move the shadow
        timer += Time.deltaTime; // frame time increment
        float u = Mathf.Clamp01(timer / Mathf.Max(0.0001f, duration)); // Normalized time (0 to 1)
        transform.position = Vector3.Lerp(fromPos, toPos, u);

        // 2. Determine target alpha based on moon mask
        float targetAlpha = offAlpha;
        if (IsPositionOverMoon(transform.position))
        {
            targetAlpha = onAlpha;
        }

        // 3. Smoothly change the shadow's alpha
        Color currentColor = sr.color;
        currentColor.a = Mathf.MoveTowards(currentColor.a, targetAlpha, alphaSmoothSpeed * Time.deltaTime);
        sr.color = currentColor;

        // 4. Check if movement is finished
        if (u >= 1f)
        {
            moving = false; // Stop moving
            // Optional: You could fire an event or reset here if needed
            OnEclipseAnimationComplete?.Invoke();
            Debug.Log("SimpleLunarShadow: Eclipse animation finished!");
        }
    }

    /// <summary>
    /// Starts the shadow movement after a specified delay.
    /// Call this from another script (e.g., your player script).
    /// </summary>
    /// <param name="delay">Time in seconds before movement begins.</param>
    public void PlayDelayed(float delay)
    {
        StartCoroutine(PlayAfter(delay));
    }

    private IEnumerator PlayAfter(float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        // Resolve actual start and end positions at the moment of playing
        // (in case startPoint/endPoint Transforms moved)
        fromPos = (startPoint != null) ? startPoint.position : transform.position;
        toPos = (endPoint != null) ? endPoint.position : transform.position;

        timer = 0f;
        moving = true; // Start movement
    }

    private void SetAlpha(float alpha)
    {
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    /// <summary>
    /// Checks if a given world position maps to a "moon" pixel on the background's moonMask.
    /// </summary>
    /// <param name="worldPos">The world position to check (e.g., the center of the shadow).</param>
    /// <returns>True if the position is over a "moon" part of the mask, false otherwise.</returns>
    private bool IsPositionOverMoon(Vector3 worldPos)
    {
        if (background == null || moonMask == null || !moonMask.isReadable)
        {
            return false; // Can't check if mask or background is missing/unreadable
        }

        // 1. Convert world position to local position of the background sprite
        // This tells us where 'worldPos' is relative to the background's pivot.
        Vector3 localBackgroundPosition = background.transform.InverseTransformPoint(worldPos);

        // 2. Get information about the background sprite
        Sprite bgSprite = background.sprite;
        if (bgSprite == null) return false;

        Rect spriteRect = bgSprite.rect;              // The sprite's rectangle within its texture (in pixels)
        Vector2 spritePivot = bgSprite.pivot;         // The sprite's pivot point (in pixels)
        float pixelsPerUnit = bgSprite.pixelsPerUnit; // How many pixels represent one Unity unit

        // 3. Convert local position (Unity units) to pixel coordinates within the sprite's texture
        // We adjust for the pivot and the PPU to get the pixel coordinate relative to the *bottom-left* of the sprite rect.
        float pixelX = localBackgroundPosition.x * pixelsPerUnit + spritePivot.x;
        float pixelY = localBackgroundPosition.y * pixelsPerUnit + spritePivot.y;

        // 4. Adjust to get pixel coordinates relative to the *moonMask's texture origin*
        // The moonMask should be aligned with the *original full texture* that the bgSprite might be a part of.
        // If your bgSprite *is* the full texture, then spriteRect.x/y will be 0.
        int maskPixelX = Mathf.RoundToInt(pixelX + spriteRect.x);
        int maskPixelY = Mathf.RoundToInt(pixelY + spriteRect.y);

        // 5. Check if these pixel coordinates are within the bounds of the moonMask texture
        if (maskPixelX < 0 || maskPixelX >= moonMask.width ||
            maskPixelY < 0 || maskPixelY >= moonMask.height)
        {
            return false; // Position is outside the moonMask texture
        }

        // 6. Get the color of the pixel at these coordinates in the moonMask
        Color pixelColor = moonMask.GetPixel(maskPixelX, maskPixelY);

        // 7. Check if the pixel is "white enough" (i.e., part of the moon)
        // We use the red channel as an indicator of brightness for a black/white mask.
        return pixelColor.r >= maskThreshold;
    }
}