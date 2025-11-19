using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Level3InstructionsTyper : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI textLabel;        // main instructions text
    public TextMeshProUGUI continueLabel;    // "Press any key to continue" text (optional)
    public AudioSource typingAudio;          // audio source with typing sound

    [Header("Typing Settings")]
    [TextArea(3, 10)]
    public string fullText;                 // what will be typed out
    public float startDelay = 0.5f;         // small delay before typing starts
    public float characterDelay = 0.03f;    // time between characters
    public float afterTextDelay = 0.5f;     // delay before showing continue text

    private bool _isTyping = false;
    private bool _finished = false;

    private void Start()
    {
        if (textLabel == null)
            textLabel = GetComponent<TextMeshProUGUI>();

        if (continueLabel != null)
            continueLabel.gameObject.SetActive(false);

        StartCoroutine(TypeRoutine());
    }

    private IEnumerator TypeRoutine()
    {
        _isTyping = true;

        // Clear initial text, wait briefly, then start typing
        textLabel.text = "";
        yield return new WaitForSeconds(startDelay);

        foreach (char c in fullText)
        {
            textLabel.text += c;

            // play typing sound for non-space characters
            if (typingAudio != null && !char.IsWhiteSpace(c))
                typingAudio.Play();

            yield return new WaitForSeconds(characterDelay);
        }

        _isTyping = false;
        _finished = true;

        // Show "press any key" after a short delay
        if (continueLabel != null)
        {
            yield return new WaitForSeconds(afterTextDelay);
            continueLabel.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (!_finished)
            return;

        // When text is fully shown, any key closes this instructions scene
        if (Input.anyKeyDown)
        {
            // Unload only THIS additive scene
            SceneManager.UnloadSceneAsync(gameObject.scene);
            SceneManager.LoadScene("Zone5");
        }
    }
}
