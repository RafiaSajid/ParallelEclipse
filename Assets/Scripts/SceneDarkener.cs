using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal; // Needed for Light2D

using System;

public class SceneDarkener : MonoBehaviour
{
    public Light2D globalLight2D; // Assign your 2D Global Light here
    // OR: public Light directionalLight; // Assign your 3D Directional Light here

    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.1f, 0.1f, 0.2f); // Dark blue/grey
    public float dayIntensity = 1.0f;
    public float nightIntensity = 0.05f; // Or even 0.05f for very dark
    public float darkenDuration = 2.0f; // How long it takes to darken

    private Color initialLightColor;
    private float initialLightIntensity;
    private Coroutine darkenCoroutine;

    public static event Action OnSceneDarkenedComplete;
    // Call this method from your GameController or SimpleLunarShadow

    void OnEnable()
    {
        LunarShadowMover.OnEclipseAnimationComplete += HandleEclipseComplete;
    }

    void OnDisable()
    {
        LunarShadowMover.OnEclipseAnimationComplete -= HandleEclipseComplete;
    }

    // --- NEW: Event Handler Method ---
    // This method will be called when OnEclipseAnimationComplete is triggered
    private void HandleEclipseComplete()
    {
        Debug.Log("SceneDarkener: Received eclipse complete notification. Starting scene darken.");
        StartDarkeningScene();
    }

    public void StartDarkeningScene()
    {
        if (globalLight2D == null)
        {
            Debug.LogWarning("Global Light 2D not assigned to SceneDarkener.");
            return;
        }

        // Store initial light settings if you want to fade back to day later
        initialLightColor = globalLight2D.color;
        initialLightIntensity = globalLight2D.intensity;

        if (darkenCoroutine != null)
        {
            StopCoroutine(darkenCoroutine);
        }
        darkenCoroutine = StartCoroutine(DarkenSceneRoutine());
    }

    private IEnumerator DarkenSceneRoutine()
    {
        float timer = 0f;
        while (timer < darkenDuration)
        {
            timer += Time.deltaTime;
            float t = timer / darkenDuration; // Normalized time (0 to 1)

            // Lerp between day and night settings
            globalLight2D.color = Color.Lerp(dayColor, nightColor, t);
            globalLight2D.intensity = Mathf.Lerp(dayIntensity, nightIntensity, t);

            yield return null; // Wait for the next frame
        }

        // Ensure it's exactly at night settings at the end
        globalLight2D.color = nightColor;
        globalLight2D.intensity = nightIntensity;

        // --- NEW: Trigger the Event when darkening is complete ---
        OnSceneDarkenedComplete?.Invoke();
        Debug.Log("SceneDarkener: Scene darkening finished!");
    }

    // Optional: Call this to revert to day
    public void RevertToDay()
    {
        // You'd need a similar routine to fade back
        // For now, just instant revert
        globalLight2D.color = initialLightColor;
        globalLight2D.intensity = initialLightIntensity;
    }

}