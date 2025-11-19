using UnityEngine;
using ClearSky;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // First try to find a playerController on the collider or any parent.
        var player = col.GetComponentInParent<playerController>();
        if (player == null && col.attachedRigidbody != null)
        {
            player = col.attachedRigidbody.GetComponent<playerController>();
        }

        if (player != null)
        {
            // Use AddHealth method (player implements it)
            player.AddHealth(healAmount);
            Debug.Log($"Picked up +{healAmount} HP. New HP: {player.CurrentHealth}");
            Debug.Log($"Pickup triggered by collider {col.name}, root {col.transform.root.name}");

            Destroy(gameObject);
        }
        // else: the collider that entered wasn't the player or its child
    }
}
