using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneController : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the next scene to load after intro")]
    public string nextSceneName = "MainMenu";

    [Header("Timing")]
    [Tooltip("Time in seconds before switching to the next scene")]
    public float delayBeforeLoad = 8f;

    [Header("Audio")]
    [Tooltip("Audio to play with the intro title")]
    public AudioSource introAudio; // Attach your audio source here

    void Start()
    {
        // Play intro audio (if assigned)
        if (introAudio != null)
            introAudio.Play();

        // Start countdown to load next scene
        Invoke("LoadNextScene", delayBeforeLoad);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
