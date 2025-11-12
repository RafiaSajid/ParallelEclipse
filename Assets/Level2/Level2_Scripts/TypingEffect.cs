using System.Collections;
using TMPro;
using UnityEngine;

public class TypingEffect : MonoBehaviour
{
    public TMP_Text textUI;
    [TextArea] public string fullText;
    public float typingSpeed = 0.05f;
    private AudioSource typingSound;

    void Start()
    {
        typingSound = GetComponent<AudioSource>();
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        textUI.text = "";
        foreach (char c in fullText)
        {
            textUI.text += c;
            if (typingSound) typingSound.Play();
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
