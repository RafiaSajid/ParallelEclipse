using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class NeonFlicker : MonoBehaviour
{
    private TMP_Text neonText;
    private AudioSource neonAudio;

    private float baseAlpha;
    public AudioClip flickerSound;   // Optional short crackle sound
    public bool playFlickerSound = true;

    void Start()
    {
        neonText = GetComponent<TMP_Text>();
        neonAudio = GetComponent<AudioSource>();
        baseAlpha = neonText.color.a;

        // Make sure the base hum plays in a loop
        if (!neonAudio.isPlaying)
        {
            neonAudio.loop = true;
            neonAudio.volume = 0.15f;  // soft hum
            neonAudio.Play();
        }

        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            float wait = Random.Range(0.05f, 0.3f);
            yield return new WaitForSeconds(wait);

            // Random intensity for flicker
            float flickerAlpha = Random.Range(0.4f, 1f);
            Color c = neonText.color;
            c.a = flickerAlpha;
            neonText.color = c;

            // Optional crackle sound each flicker
            if (playFlickerSound && flickerSound != null)
            {
                // Randomize pitch slightly for realism
                neonAudio.pitch = Random.Range(0.9f, 1.2f);
                neonAudio.PlayOneShot(flickerSound, Random.Range(0.05f, 0.15f));
            }
        }
    }
}
