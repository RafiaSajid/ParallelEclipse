using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
/// <summary>
/// Handles this portal's audio behavior (proximity-based sound)
/// and notifies the PortalManager when the player enters.
/// </summary>
public class TeleportalController : MonoBehaviour
{
    [HideInInspector]
    public bool isCorrectPortal;

    [Header("Sound Settings")]
    [Tooltip("How close the player must be to hear this portal's sound.")]
    public float soundRadius = 4f;

    private AudioSource _audioSource;
    private PortalManager _manager;
    private Transform _player;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;

        // Make sure OnTriggerEnter2D actually fires
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    /// <summary>
    /// Called by PortalManager after the portal is created.
    /// </summary>
    public void Setup(PortalManager manager, bool correct, AudioClip clip, Transform player)
    {
        _manager = manager;
        isCorrectPortal = correct;
        _player = player;

        _audioSource.clip = clip;
        // Do NOT play here – we play/stop based on distance in Update().
    }

    private void Update()
    {
        if (_player == null || _audioSource.clip == null)
            return;

        float distance = Vector2.Distance(_player.position, transform.position);

        // Player close enough → play sound
        if (distance <= soundRadius)
        {
            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }
        // Player far → stop sound
        else
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to the player collider
        if (!other.CompareTag("Player"))
            return;

        if (_manager != null)
        {
            _manager.PlayerEnteredPortal(this);
        }
        else
        {
            Debug.LogWarning("TeleportalController: Manager is not set on " + name);
        }
    }
}
