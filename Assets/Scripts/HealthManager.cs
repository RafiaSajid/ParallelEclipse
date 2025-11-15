using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ClearSky;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int hpPerHeart = 20;
    [SerializeField] private List<Image> hearts = new List<Image>();

    private playerController player;

    private void Start()
    {
        player = FindObjectOfType<playerController>();
        UpdateHeartsInstant();
    }

    // instant update
    public void UpdateHeartsInstant()
    {
        int hp = player.CurrentHealth;
        int heartsToShow = hp / hpPerHeart;

        for (int i = 0; i < hearts.Count; i++)
            hearts[i].gameObject.SetActive(i < heartsToShow);
    }

    // flashy update when losing heart
    public void AnimateHeartLoss()
    {
        StartCoroutine(FlashLostHearts());
    }

    private IEnumerator FlashLostHearts()
    {
        int hp = player.CurrentHealth;
        int heartsToShow = hp / hpPerHeart;

        for (int i = heartsToShow; i < hearts.Count; i++)
        {
            if (hearts[i].gameObject.activeSelf)
            {
                // Flash animation
                Image heart = hearts[i];
                for (int f = 0; f < 4; f++)
                {
                    heart.enabled = false;
                    yield return new WaitForSeconds(0.1f);
                    heart.enabled = true;
                    yield return new WaitForSeconds(0.1f);
                }

                heart.gameObject.SetActive(false);
            }
        }
    }

    // flashy update when adding heart
    public void AnimateHeartGain()
    {
        StartCoroutine(FlashGain());
    }

    private IEnumerator FlashGain()
    {
        int hp = player.CurrentHealth;
        int heartsToShow = hp / hpPerHeart;

        for (int i = 0; i < heartsToShow; i++)
        {
            if (!hearts[i].gameObject.activeSelf)
            {
                Image h = hearts[i];

                for (int f = 0; f < 4; f++)
                {
                    h.enabled = false;
                    yield return new WaitForSeconds(0.1f);
                    h.enabled = true;
                    yield return new WaitForSeconds(0.1f);
                }

                h.gameObject.SetActive(true);
            }
        }
    }
}
