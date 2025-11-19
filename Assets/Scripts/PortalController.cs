using System;
using System.Collections;
using UnityEngine;
#if UNITY_URP_2D
using UnityEngine.Rendering.Universal;  
#endif

public class PortalController : MonoBehaviour
{
    public SpriteRenderer portalRenderer;         // assign PortalSprite  
#if UNITY_URP_2D
    public Light2D glowLight2D;                   // URP 2D light (optional)  
#endif
    // public Light glowLight3D;                     // 3D light (optional)  
    public float glowMaxIntensity = 3f;
    public Color glowColor = Color.cyan;
    public float glowFadeDuration = 1.5f;

    public Transform pullTarget;                  // portal center (where player will be pulled to)  
    public float pullDelayAfterGlow = 0.5f;
    public float pullDuration = 1.5f;

    // Events  
    public static event Action OnPortalActivated;
    public static event Action OnPortalPullComplete;

    void Awake()
    {
        if (portalRenderer != null) portalRenderer.enabled = false;
#if UNITY_URP_2D
        if (glowLight2D != null) glowLight2D.intensity = 0f;  
#endif
        // if (glowLight3D != null) glowLight3D.intensity = 0f;  
    }

    void OnEnable()
    {
        SceneDarkener.OnSceneDarkenedComplete += HandleSceneDarkened;
    }

    void OnDisable()
    {
        SceneDarkener.OnSceneDarkenedComplete -= HandleSceneDarkened;
    }

    private void HandleSceneDarkened()
    {
        StartCoroutine(PortalActivationSequence());
    }

    private IEnumerator PortalActivationSequence()
    {
        // 1) Make portal visible (fade-in)  
        if (portalRenderer != null)
        {
            portalRenderer.enabled = true;
            Color c = portalRenderer.color;
            c.a = 0f;
            portalRenderer.color = c;

            float fadeDur = 0.6f;
            float ft = 0f;
            while (ft < fadeDur)
            {
                ft += Time.deltaTime;
                float u = ft / fadeDur;
                c.a = Mathf.Lerp(0f, 1f, u);
                portalRenderer.color = c;
                yield return null;
            }
            c.a = 1f;
            portalRenderer.color = c;
        }

        // 2) Glow up  
        float timer = 0f;
#if UNITY_URP_2D
        if (glowLight2D != null) glowLight2D.color = glowColor;  
#endif
        // if (glowLight3D != null) glowLight3D.color = glowColor;  

        while (timer < glowFadeDuration)
        {
            timer += Time.deltaTime;
            float u = timer / glowFadeDuration;
#if UNITY_URP_2D
            if (glowLight2D != null) glowLight2D.intensity = Mathf.Lerp(0f, glowMaxIntensity, u);  
#endif
            // if (glowLight3D != null) glowLight3D.intensity = Mathf.Lerp(0f, glowMaxIntensity, u);  
            yield return null;
        }
#if UNITY_URP_2D
        if (glowLight2D != null) glowLight2D.intensity = glowMaxIntensity;  
#endif
        // if (glowLight3D != null) glowLight3D.intensity = glowMaxIntensity;  

        // Notify that portal is activated (others can respond e.g., play sound)  
        OnPortalActivated?.Invoke();

        // 3) Optional small delay then start pulling player  
        if (pullDelayAfterGlow > 0f) yield return new WaitForSeconds(pullDelayAfterGlow);

        // Pull player to portal center


        // 4) Portal pull complete -> notify and optionally play an effect
        OnPortalPullComplete?.Invoke();

        // Optionally: small delay to allow final frame to show then let GameController load next scene
        yield return null;
    }
}
//}