using UnityEngine;
using System.Collections; // Required for Coroutines

// Rename the script file to HealthPotionSpawner.cs
public class HealthSpawner : MonoBehaviour
{
    // The prefab of the potion (drag your Potion Prefab here)
    [SerializeField] private GameObject healthPrefab;

    // Time interval between spawns
    public float spawnInterval = 25f;

    // Define the boundaries for random positioning
    public Vector2 spawnRangeX = new Vector2(8f, 20f);
    public Vector2 spawnRangeY = new Vector2(1f, 3f);

    void Start()
    {
        if (healthPrefab == null)
        {
            Debug.LogError("Health Prefab is not assigned! Cannot spawn.");
            return;
        }
        // Start the continuous spawning loop
        StartCoroutine(SpawnHealthPotions());
    }

    // Coroutine to handle timed spawning
    IEnumerator SpawnHealthPotions()
    {
        while (true) // Infinite loop for continuous spawning
        {
            // 1. Wait for the defined interval
            yield return new WaitForSeconds(spawnInterval);

            // 2. Calculate a random position
            Vector3 randomPosition = new Vector3(
                Random.Range(spawnRangeX.x, spawnRangeX.y),
                Random.Range(spawnRangeY.x, spawnRangeY.y),
                0f // Assuming a 2D game
            );

            // 3. Instantiate the potion at the random position
            // The position is relative to the world, not the spawner's transform
            Instantiate(healthPrefab, randomPosition, Quaternion.identity);

            Debug.Log("Health Potion spawned at: " + randomPosition);
        }
    }
}