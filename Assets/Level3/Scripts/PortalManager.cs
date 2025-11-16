using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns portals, assigns audio, and handles teleport behavior.
/// Creation details and selection strategy are delegated to helper abstractions.
/// </summary>
public class PortalManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject portalPrefab;       // Teleportal prefab
    [SerializeField] private List<Transform> spawnPoints;   // Portal spawn positions
    [SerializeField] private PortalAudioSet audioSet;       // Earth + space sounds

    [Header("Characters")]
    [SerializeField] private Transform PlayerPrefab ;              // Player transform
    [SerializeField] private Transform npc;                 // NPC transform that follows player

    [Header("Teleport Targets")]
    [SerializeField] private Transform realWorldExit;       // Where correct portal sends them
    [SerializeField] private Vector2 exitOffsetFromPortal = new Vector2(0f, 1f);

    private readonly List<TeleportalController> _portals = new List<TeleportalController>();
    private TeleportalController _correctPortal;

    // Factory + Strategy
    private IPortalFactory _portalFactory;
    private ICorrectPortalSelector _correctPortalSelector;

    private void Awake()
    {
        // Build concrete helpers
        _portalFactory = new PrefabPortalFactory(portalPrefab);
        _correctPortalSelector = new RandomCorrectPortalSelector();
    }

    private void Start()
    {
        
        SpawnPortals();
    }

    private void SpawnPortals()
    {
        _portals.Clear();

        

        // Create portals via factory
        for (int i = 0; i < 5; i++)
{
    Vector3 randomPos = new Vector3(
        Random.Range(-37f, 11f),
        Random.Range(2f, 4.4f),
        0f
    );

    GameObject spawn = new GameObject("RandomSpawnPoint_" + i);
    //this creates a new empty GameObject in the scene.

//It basically simulates what you would manually create with:

//Right-click → Create Empty
    spawn.transform.position = randomPos;

    spawnPoints.Add(spawn.transform);
}

        
        foreach (Transform sp in spawnPoints)
        {
            TeleportalController portal = _portalFactory.CreatePortal(sp.position);
            if (portal != null)


            {
                _portals.Add(portal);
            }
            else
            {
                Debug.LogWarning("PortalManager: Failed to create portal at " + sp.name);
            }
        }

        if (_portals.Count == 0)
        {
            Debug.LogWarning("PortalManager: No portals were created.");
            return;
        }

        // Strategy: choose which portal is correct
        _correctPortal = _correctPortalSelector.SelectCorrectPortal(_portals);

        // Assign correct/wrong audio to each portal
        AssignPortalAudio();
    }

    /// <summary>
    /// Assigns audio and configuration to each portal.
    /// </summary>
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

    /// <summary>
    /// Called by a portal when the player enters it.
    /// </summary>
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
        else
            Debug.LogWarning("PortalManager: Player transform is not assigned.");

        if (npc != null)
            npc.position = pos + (Vector3)exitOffsetFromPortal;

        Debug.Log("Correct portal! Back to real world.");
    }

    private void TeleportToRandomOtherPortal(TeleportalController enteredPortal)
    {
        if (_portals.Count <= 1 || enteredPortal == null)
            return;

        // Choose from portals that are not the one we just entered
        List<TeleportalController> others = new List<TeleportalController>(_portals);
        others.Remove(enteredPortal);

        if (others.Count == 0)
            return;

        TeleportalController destinationPortal = others[Random.Range(0, others.Count)];

        Vector3 pos = destinationPortal.transform.position + (Vector3)exitOffsetFromPortal;

        if (PlayerPrefab != null)
            PlayerPrefab.position = pos;
        else
            Debug.LogWarning("PortalManager: Player transform is not assigned.");

        if (npc != null)
            npc.position = pos;

        Debug.Log("Wrong portal! Looped to another one.");
    }
}

/// --------- FACTORY + STRATEGY IMPLEMENTATIONS ----------

public interface IPortalFactory
{
    TeleportalController CreatePortal(Vector3 position);
}

public class PrefabPortalFactory : IPortalFactory
{
    private readonly GameObject _portalPrefab;

    public PrefabPortalFactory(GameObject portalPrefab)
    {
        _portalPrefab = portalPrefab;
    }

    public TeleportalController CreatePortal(Vector3 position)
    {
        if (_portalPrefab == null)
        {
            Debug.LogWarning("PrefabPortalFactory: Portal prefab is not assigned.");
            return null;
        }

        GameObject portalGO = Object.Instantiate(_portalPrefab, position, Quaternion.identity);
        TeleportalController portal = portalGO.GetComponent<TeleportalController>();

        if (portal == null)
        {
            Debug.LogWarning("PrefabPortalFactory: Portal prefab is missing TeleportalController component.");
        }

        return portal;
    }
}

public interface ICorrectPortalSelector
{
    TeleportalController SelectCorrectPortal(IReadOnlyList<TeleportalController> portals);
}

public class RandomCorrectPortalSelector : ICorrectPortalSelector
{
    public TeleportalController SelectCorrectPortal(IReadOnlyList<TeleportalController> portals)
    {
        if (portals == null || portals.Count == 0)
            return null;

        int index = Random.Range(0, portals.Count);
        return portals[index];
    }
}
