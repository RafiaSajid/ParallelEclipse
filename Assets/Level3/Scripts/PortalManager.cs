using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private PortalAudioSet audioSet;

    [Header("Portal Spawn Area (World Space)")]
    [SerializeField] private float spawnMinX = -37f;
    [SerializeField] private float spawnMaxX = 11f;
    [SerializeField] private float spawnMinY = 2f;
    [SerializeField] private float spawnMaxY = 4.4f;

    [Header("Portal Settings")]
    [SerializeField] private int portalCount = 5;            // 5–6 portals
    [SerializeField] private float minPortalDistance = 3f;   // distance between portals
    [SerializeField] private int maxTriesPerPortal = 50;     // safety for random search

    [Header("Characters")]
    [SerializeField] private Transform PlayerPrefab;   // actually your player instance
    [SerializeField] private Transform npc;

    [Header("Teleport Targets")]
    [SerializeField] private Transform realWorldExit;
    [SerializeField] private Vector2 exitOffsetFromPortal = new Vector2(0f, 1f);

    private readonly List<TeleportalController> _portals = new List<TeleportalController>();
    private readonly List<Vector3> _spawnPositions = new List<Vector3>();

    private TeleportalController _correctPortal;

    private void Start()
    {
        SpawnPortals();
    }

    // Optional: shows the spawn area as a yellow box when the object is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        float centerX = (spawnMinX + spawnMaxX) * 0.5f;
        float centerY = (spawnMinY + spawnMaxY) * 0.5f;
        float sizeX = Mathf.Abs(spawnMaxX - spawnMinX);
        float sizeY = Mathf.Abs(spawnMaxY - spawnMinY);

        Vector3 center = new Vector3(centerX, centerY, 0f);
        Vector3 size = new Vector3(sizeX, sizeY, 0f);

        Gizmos.DrawWireCube(center, size);
    }

    private void SpawnPortals()
    {
        _portals.Clear();
        _spawnPositions.Clear();

        if (portalPrefab == null)
        {
            Debug.LogError("PortalManager: portalPrefab is not assigned.");
            return;
        }

        if (spawnMaxX <= spawnMinX || spawnMaxY <= spawnMinY)
        {
            Debug.LogError("PortalManager: Spawn area values are invalid. Check spawnMin/Max X/Y.");
            return;
        }

        // Pick portalCount random positions inside the spawn rectangle, with distance constraint
        for (int i = 0; i < portalCount; i++)
        {
            bool foundSpot = false;

            for (int tries = 0; tries < maxTriesPerPortal; tries++)
            {
                // Random position in your chosen world-space area
                float x = Random.Range(spawnMinX, spawnMaxX);
                float y = Random.Range(spawnMinY, spawnMaxY);
                Vector3 worldPos = new Vector3(x, y, 0f);

                // Check distance to previously chosen positions
                bool tooClose = false;
                foreach (Vector3 pos in _spawnPositions)
                {
                    if (Vector3.Distance(worldPos, pos) < minPortalDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose)
                    continue;

                // Good position
                _spawnPositions.Add(worldPos);
                foundSpot = true;
                break;
            }

            if (!foundSpot)
            {
                Debug.LogWarning($"PortalManager: Could not find valid spot for portal {i}");
            }
        }

        // Instantiate portals at chosen positions
        foreach (Vector3 pos in _spawnPositions)
        {
            TeleportalController portal =
                Instantiate(portalPrefab, pos, Quaternion.identity)
                .GetComponent<TeleportalController>();

            if (portal != null)
            {
                _portals.Add(portal);
            }
            else
            {
                Debug.LogWarning("PortalManager: Failed to create portal - no TeleportalController on prefab?");
            }
        }

        if (_portals.Count == 0)
        {
            Debug.LogWarning("PortalManager: No portals were created.");
            return;
        }

        // Pick one portal as the correct one
        _correctPortal = _portals[Random.Range(0, _portals.Count)];

        AssignPortalAudio();
    }

    private void AssignPortalAudio()
    {
        if (audioSet == null)
        {
            Debug.LogWarning("PortalManager: No audio set assigned.");
            return;
        }

        if (PlayerPrefab == null)
        {
            Debug.LogWarning("PortalManager: Player transform not assigned.");
            return;
        }

        foreach (TeleportalController portal in _portals)
        {
            if (portal == null) continue;

            bool isCorrect = (portal == _correctPortal);
            AudioClip clip = GetClipForPortal(isCorrect);

            portal.Setup(this, isCorrect, clip, PlayerPrefab);
        }
    }

    private AudioClip GetClipForPortal(bool isCorrect)
    {
        if (audioSet == null) return null;

        if (isCorrect)
        {
            if (audioSet.earthPortalClips == null || audioSet.earthPortalClips.Length == 0)
                return null;

            int idx = Random.Range(0, audioSet.earthPortalClips.Length);
            return audioSet.earthPortalClips[idx];
        }
        else
        {
            if (audioSet.spacePortalClips == null || audioSet.spacePortalClips.Length == 0)
                return null;

            int idx = Random.Range(0, audioSet.spacePortalClips.Length);
            return audioSet.spacePortalClips[idx];
        }
    }

    public void PlayerEnteredPortal(TeleportalController portal)
    {
        if (portal == null)
            return;

        if (portal == _correctPortal)
        {
            TeleportToRealWorld();
        }
        else
        {//wrong portal
            TeleportToRandomOtherPortal(portal);
        }
    }

    private void TeleportToRealWorld()
    {
        if (realWorldExit == null)
        {
            Debug.LogWarning("PortalManager: Real world exit is not set.");
            return;
        }

        Vector3 pos = realWorldExit.position;

        if (PlayerPrefab != null)
            PlayerPrefab.position = pos;

        if (npc != null)
            npc.position = pos + (Vector3)exitOffsetFromPortal;
    }

    private void TeleportToRandomOtherPortal(TeleportalController enteredPortal)
    {
        if (_portals.Count <= 1 || enteredPortal == null)
            return;

        List<TeleportalController> others = new List<TeleportalController>(_portals);
        others.Remove(enteredPortal);

        if (others.Count == 0)
            return;

        TeleportalController destinationPortal = others[Random.Range(0, others.Count)];
        if (destinationPortal == null) return;

        Vector3 pos = destinationPortal.transform.position + (Vector3)exitOffsetFromPortal;

        if (PlayerPrefab != null)
            PlayerPrefab.position = pos;

        if (npc != null)
            npc.position = pos;
    }
}

/*using System.Collections.Generic;
using UnityEngine;


public class PortalManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private PortalAudioSet audioSet;

    [Header("Portal Spawn Area (World Space)")]
[SerializeField] private float spawnMinX = -37f;
[SerializeField] private float spawnMaxX = 11f;
[SerializeField] private float spawnMinY = 2f;
[SerializeField] private float spawnMaxY = 4.4f;


    [Header("Tilemap Area for Portals")]
    [SerializeField] private int portalCount = 5;            // 5–6 portals
    [SerializeField] private float minPortalDistance = 3f;   // distance between portals
    [SerializeField] private int maxTriesPerPortal = 50;     // safety for random search

    [Header("Characters")]
    [SerializeField] private Transform PlayerPrefab;
    [SerializeField] private Transform npc;

    [Header("Teleport Targets")]
    [SerializeField] private Transform realWorldExit;
    [SerializeField] private Vector2 exitOffsetFromPortal = new Vector2(0f, 1f);

    private readonly List<TeleportalController> _portals = new List<TeleportalController>();
    private readonly List<Vector3> _spawnPositions = new List<Vector3>();

    private TeleportalController _correctPortal;

 

    private void Awake()
    {
      
    }

    private void Start()
    {
        SpawnPortals();
    }

    private void SpawnPortals()
    {
        _portals.Clear();
        _spawnPositions.Clear();

      
for (int i = 0; i < portalCount; i++)
{
    bool foundSpot = false;

    for (int tries = 0; tries < maxTriesPerPortal; tries++)
    {
        // Random position in your chosen world-space area
        float x = Random.Range(spawnMinX, spawnMaxX);
        float y = Random.Range(spawnMinY, spawnMaxY);
        Vector3 worldPos = new Vector3(x, y, 0f);

        // Check distance to previously chosen positions
        bool tooClose = false;
        foreach (Vector3 pos in _spawnPositions)
        {
            if (Vector3.Distance(worldPos, pos) < minPortalDistance)
            {
                tooClose = true;
                break;
            }
        }

        if (tooClose)
            continue;

        // Good position
        _spawnPositions.Add(worldPos);
        foundSpot = true;
        break;
    }

    if (!foundSpot)
    {
        Debug.LogWarning($"PortalManager: Could not find valid spot for portal {i}");
    }
}

        // Instantiate portals at chosen positions
        foreach (Vector3 pos in _spawnPositions)
        {
            TeleportalController portal = 
               Instantiate(portalPrefab, pos, Quaternion.identity).GetComponent<TeleportalController>();

            if (portal != null)
            {
                _portals.Add(portal);
            }
            else
            {
                Debug.LogWarning("PortalManager: Failed to create portal.");
            }
        }

        if (_portals.Count == 0)
        {
            Debug.LogWarning("PortalManager: No portals were created.");
            return;
        }

        _correctPortal = _portals[Random.Range(0, _portals.Count)];

        AssignPortalAudio();
    }

    private void AssignPortalAudio()
    {
        if (audioSet == null)
        {
            Debug.LogWarning("PortalManager: No audio set assigned.");
            return;
        }

        if (PlayerPrefab == null)
        {
            Debug.LogWarning("PortalManager: Player transform not assigned.");
            return;
        }

        foreach (TeleportalController portal in _portals)
        {
            if (portal == null) continue;

            bool isCorrect = (portal == _correctPortal);
            AudioClip clip = GetClipForPortal(isCorrect);

            portal.Setup(this, isCorrect, clip, PlayerPrefab);
        }
    }

    private AudioClip GetClipForPortal(bool isCorrect)
    {
        if (audioSet == null) return null;

        if (isCorrect)
        {
            if (audioSet.earthPortalClips == null || audioSet.earthPortalClips.Length == 0)
                return null;

            int idx = Random.Range(0, audioSet.earthPortalClips.Length);
            return audioSet.earthPortalClips[idx];
        }
        else
        {
            if (audioSet.spacePortalClips == null || audioSet.spacePortalClips.Length == 0)
                return null;

            int idx = Random.Range(0, audioSet.spacePortalClips.Length);
            return audioSet.spacePortalClips[idx];
        }
    }

    public void PlayerEnteredPortal(TeleportalController portal)
    {
        if (portal == null)
            return;

        if (portal == _correctPortal)
        {
            TeleportToRealWorld();
        }
        else
        {
            TeleportToRandomOtherPortal(portal);
        }
    }

    private void TeleportToRealWorld()
    {
        if (realWorldExit == null)
        {
            Debug.LogWarning("PortalManager: Real world exit is not set.");
            return;
        }

        Vector3 pos = realWorldExit.position;

        if (PlayerPrefab != null)
            PlayerPrefab.position = pos;

        if (npc != null)
            npc.position = pos + (Vector3)exitOffsetFromPortal;
    }

    private void TeleportToRandomOtherPortal(TeleportalController enteredPortal)
    {
        if (_portals.Count <= 1 || enteredPortal == null)
            return;

        List<TeleportalController> others = new List<TeleportalController>(_portals);
        others.Remove(enteredPortal);

        if (others.Count == 0)
            return;

        TeleportalController destinationPortal = others[Random.Range(0, others.Count)];

        Vector3 pos = destinationPortal.transform.position + (Vector3)exitOffsetFromPortal;

        if (PlayerPrefab != null)
            PlayerPrefab.position = pos;

        if (npc != null)
            npc.position = pos;
    }
}*/
