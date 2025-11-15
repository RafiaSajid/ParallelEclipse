using System.Collections;
using UnityEngine;

public class Creatures : MonoBehaviour
{
    // Prefabs of your enemies (e.g., zombies)
    [SerializeField] private GameObject zombiesPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Time before the first enemy spawns.")]
    [SerializeField] private float initialDelay = 5f; // Initial delay before spawning starts
    [Tooltip("Time between subsequent enemy spawns.")]
    [SerializeField] private float spawnInterval = 3f; // Time between spawns (to control frequency)
    [Tooltip("Total duration for which enemies will spawn.")]
    [SerializeField] private float totalSpawnDuration = 30f; // Spawning stops after 60 seconds

    [Header("Spawn Position")]
    // Min and Max x-coordinates for spawning, assuming y is fixed
    [SerializeField] private float minTras = -8f;
    [SerializeField] private float maxTras = 8f;

    private void Start()
    {
        // Start the main spawning coroutine
        StartCoroutine(StartSpawning());
    }

    private IEnumerator StartSpawning()
    {
        // 1. Wait for the initial delay before spawning begins
        yield return new WaitForSeconds(initialDelay);

        float startTime = Time.time;

        // 2. Loop until the total duration has passed
        while (Time.time < startTime + totalSpawnDuration)
        {
            // 3. Determine a random spawn position (x-coordinate)
            float wantedX = Random.Range(minTras, maxTras);
            Vector3 position = new Vector3(wantedX, transform.position.y, transform.position.z);

            // 4. Select a random enemy prefab
            GameObject prefabToSpawn = zombiesPrefab;

            // 5. Instantiate the enemy
            // The enemy's lifecycle (movement, combat, death) is handled by EnemyController
            Instantiate(prefabToSpawn, position, Quaternion.identity);

            // 6. Wait for the set interval before spawning the next enemy
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("Enemy spawning time has ended.");
    }
}














// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// public class Creatures : MonoBehaviour
// {       
// [SerializeField] private GameObject [] zombiesPrefab;
//     float delay = 12f; // delay time before zombies appear
//     [SerializeField]private float secondSpawn = 0.5f;
//     [SerializeField]private float minTras;
//     [SerializeField]private float maxTras;

//     private Animator anim;
//     // Start is called before the first frame update
//     void Start()
//     {
//         //anim = GetComponenet<Animator>();
//         StartCoroutine(ZombieSpawn());

//     }

//     // Update is called once per frame
//     void Update()
//     {   
//         //Vector3 randomindex = new Vector3 Random.Range(0, zombiesPrefab.Length);
//         delay -= 2* Time.deltaTime;
//     }
//         IEnumerator ZombieSpawn()
//         {   
//             //if(delay <= 0 ){
//             while(true)
//             {
//                 var wanted = Random.Range(minTras,maxTras);
//                 var position = new Vector3(wanted, transform.position.y);
//                 GameObject creatureObj = Instantiate(zombiesPrefab[Random.Range(0, zombiesPrefab.Length)],position, Quaternion.identity);
//                 yield return new WaitForSeconds(secondSpawn);
//                 Destroy(creatureObj, 1f);
//             }
//            // }
//         }

//     }





//  // int randomindex = Random.Range(0, zombies.Length);
//         // if(delay >= 12 )
//         // {
//         //     Vector3 randomSpawnPosition = new Vector3(Random.Range(12,0));
//         // }
