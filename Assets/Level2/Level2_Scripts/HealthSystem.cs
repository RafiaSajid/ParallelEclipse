using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public Image healthFill;
    public TMP_Text healthText;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void ReduceHealth()
    {
        currentHealth--;
        if (currentHealth < 0) currentHealth = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        healthFill.fillAmount = (float)currentHealth / maxHealth;
        healthText.text = "Health: " + currentHealth;
    }

    public bool IsDead() => currentHealth <= 0;
}
