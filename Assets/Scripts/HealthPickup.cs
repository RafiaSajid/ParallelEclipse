using UnityEngine;
using ClearSky;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] GameObject HealthPrefab;
    public int healAmount = 20;


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Health pickup triggered");
            playerController player = col.GetComponent<playerController>();

            if (player != null)
            {
                player.AddHealth(healAmount);
                Debug.Log("Picked up +" + healAmount + " HP");
            }

            Destroy(gameObject);
        }
    }
}
