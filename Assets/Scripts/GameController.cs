using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("Eclipse")]
    public LunarShadowMover moonShadowEffect; // Drag your black-circle GameObject here in Inspector
    public float timeToStartEclipse = 5f; // Delay before the shadow starts moving

    [Header("Scene Darkening")]
    public SceneDarkener sceneDark; // optional reference if you want GameController to trigger darkening itself

    [Header("Portal / Transition")]
    public string nextSceneName = "Post-apocalyptic Scene"; // Set this in the Inspector!
    public float delayBeforeLoadingNextScene = 1.0f; // wait after pull completes before loading
    public float optionalFadeDuration = 0.6f; // if using a fade manager or UI fade, use this value

    void OnEnable()
    {
        // subscribe to eclipse complete so we can optionally trigger scene darkening
        LunarShadowMover.OnEclipseAnimationComplete += HandleEclipseAnimationComplete;

        // subscribe to the SceneDarkener completion (if you still want to react to it)
        SceneDarkener.OnSceneDarkenedComplete += HandleSceneDarkenedComplete;

        // subscribe to portal completion (PortalController triggers when player pulled)
        PortalController.OnPortalPullComplete += HandlePortalPullComplete;
    }

    void OnDisable()
    {
        LunarShadowMover.OnEclipseAnimationComplete -= HandleEclipseAnimationComplete;
        SceneDarkener.OnSceneDarkenedComplete -= HandleSceneDarkenedComplete;
        PortalController.OnPortalPullComplete -= HandlePortalPullComplete;
    }

    void Start()
    {
        if (moonShadowEffect != null)
        {
            Debug.Log($"Eclipse will start in {timeToStartEclipse} seconds.");
            moonShadowEffect.PlayDelayed(timeToStartEclipse);
        }
        else
        {
            Debug.LogWarning("moonShadowEffect not assigned in GameController. Eclipse won't start.");
        }
    }

    // If you prefer GameController to explicitly tell SceneDarkener to start (optional)
    private void HandleEclipseAnimationComplete()
    {
        Debug.Log("GameController: Eclipse animation complete.");

        // Option A: Let SceneDarkener already be subscribed to LunarShadowMover and handle itself.
        // Option B (optional): if you want GameController to explicitly start darkening:
        if (sceneDark != null)
        {
            Debug.Log("GameController: Triggering SceneDarkener.StartDarkeningScene() (optional).");
            sceneDark.StartDarkeningScene();
        }
    }

    // This is still kept if you want to do something right after scene gets dark,
    // but we won't load the next scene here — wait for portal pull completion instead.
    private void HandleSceneDarkenedComplete()
    {
        Debug.Log("GameController: Scene darkened complete. Waiting for portal sequence.");
        // You can start portal (if PortalController isn't already listening to this event).
        // Example (if you want GameController to trigger portal explicitly):
        // if (portalController != null) portalController.ActivatePortal();
    }

    // Called when PortalController signals the player pull is complete
    private void HandlePortalPullComplete()
    {
        Debug.Log("GameController: Portal pull complete. Preparing to load next scene.");
        StartCoroutine(LoadNextSceneWithDelay(delayBeforeLoadingNextScene));
    }

    private IEnumerator LoadNextSceneWithDelay(float delay)
    {
        if (optionalFadeDuration > 0f)
        {
            // Optional: start a UI fade out here if you have a FadeManager or CanvasGroup
            // Example:
            // if (FadeManager.Instance != null) yield return StartCoroutine(FadeManager.Instance.FadeOut(optionalFadeDuration));
            // Otherwise just wait the fade duration.
            yield return new WaitForSeconds(optionalFadeDuration);
        }

        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"GameController: Loading scene '{nextSceneName}'.");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("GameController: nextSceneName is not set. Cannot load next scene.");
        }
    }
}











// using UnityEngine;
// using UnityEngine.SceneManagement;
// using System.Collections;

// public class GameController : MonoBehaviour
// {
//     public LunarShadowMover moonShadowEffect; // Drag your black-circle GameObject here in Inspector
//     public float timeToStartEclipse = 5f; // Delay before the shadow starts moving
//     public SceneDarkener sceneDark;
//     public float delayBeforeLoadingNextScene = 1.0f; // Optional: wait a moment before loading
    
//     public string nextSceneName = "Post-apocalyptic Scene"; // Set this in the Inspector!

//     void OnEnable()
//     {
//         // Subscribe to the event from SimpleLunarShadow
//         LunarShadowMover.OnEclipseAnimationComplete += HandleEclipseAnimationComplete;
//         // Subscribe to the event from SceneDarkener
//         SceneDarkener.OnSceneDarkenedComplete += HandleSceneDarkenedComplete;
//     }

//     void OnDisable()
//     {
//         // Unsubscribe to prevent memory leaks or errors
//         LunarShadowMover.OnEclipseAnimationComplete -= HandleEclipseAnimationComplete;
//         SceneDarkener.OnSceneDarkenedComplete -= HandleSceneDarkenedComplete;
//     }

    

//     void Start()
//     {
//         if (moonShadowEffect != null)
//         {
//             Debug.Log($"Eclipse will start in {timeToStartEclipse} seconds.");
//             moonShadowEffect.PlayDelayed(timeToStartEclipse);
//         }
//         else
//         {
//             Debug.LogWarning("moonShadowEffect not assigned in GameController. Eclipse won't start.");
//         }
//     }
//     private IEnumerator LoadNextSceneWithDelay(float delay)
//     {
//         if (delay > 0f)
//         {
//             yield return new WaitForSeconds(delay);
//         }

//         if (!string.IsNullOrEmpty(nextSceneName))
//         {
//             SceneManager.LoadScene(nextSceneName);
//         }
//         else
//         {
//             Debug.LogWarning("GameController: nextSceneName is not set. Cannot load next scene.");
//         }
//     }
//     private void HandleEclipseAnimationComplete()
//     {
//         Debug.Log("GameController: Received eclipse animation complete. (SceneDarkener should handle darkening itself)");
//         // In this setup, SceneDarkener *already* subscribes to this event directly,
//         // so GameController doesn't need to explicitly tell it to darken.
//         // This log just confirms GameController also knows.
//     }

//     // This method is called when the scene darkening is finished
//     private void HandleSceneDarkenedComplete()
//     {
//         Debug.Log("GameController: Received scene darkened complete. Loading next scene.");
//         StartCoroutine(LoadNextSceneWithDelay(delayBeforeLoadingNextScene));
//     }
//     // Assuming you want the scene to start darkening as the eclipse begins
//     // if (sceneDark != null)
//     // {
//     //      // Start darkening slightly before or at the same time as the shadow starts moving
//     //      // You might want to adjust the delay here based on when the shadow is actually over the moon.
//     //     sceneDark.StartDarkeningScene();
//     // }
    

 
// }

