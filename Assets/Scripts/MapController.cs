using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string mapSceneName = "MapAdditive";

    // We use a flag to stop player from spamming keys while loading
    private bool isProcessing = false;
    private bool isMapOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            // Only allow input if we aren't currently loading/unloading
            if (!isProcessing)
            {
                ToggleMap();
            }
        }
    }

    private void ToggleMap()
    {
        if (isMapOpen)
        {
            StartCoroutine(UnloadMapSequence());
        }
        else
        {
            StartCoroutine(LoadMapSequence());
        }
    }

    private IEnumerator LoadMapSequence()
    {
        isProcessing = true; // Lock input

        // Check if scene is already loaded to be safe
        Scene existingScene = SceneManager.GetSceneByName(mapSceneName);
        if (!existingScene.isLoaded)
        {
            // Load the scene additively (Overlay)
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mapSceneName, LoadSceneMode.Additive);

            // Wait until it is fully loaded
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        isMapOpen = true;
        isProcessing = false; // Unlock input
        Debug.Log("Map Opened");
    }

    private IEnumerator UnloadMapSequence()
    {
        isProcessing = true; // Lock input

        // 1. Get the scene object by its name
        Scene mapScene = SceneManager.GetSceneByName(mapSceneName);

        // 2. CRITICAL CHECK: Only proceed if the scene is actually loaded.
        if (mapScene.isLoaded)
        {
            Debug.Log("Attempting to unload scene: " + mapSceneName);

            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(mapSceneName);

            // Wait until it is fully unloaded
            if (asyncUnload != null)
            {
                while (!asyncUnload.isDone)
                {
                    yield return null;
                }
            }
        }
        else
        {
            Debug.LogWarning("Attempted to unload map, but it was not loaded. Scene state likely desynced.");
        }

        isMapOpen = false;
        isProcessing = false; // Unlock input
        Debug.Log("Map Closed successfully.");
    }
}