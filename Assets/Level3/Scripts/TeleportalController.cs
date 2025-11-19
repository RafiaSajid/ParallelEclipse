using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class TeleportalController : MonoBehaviour
{
    [HideInInspector]
    public bool isCorrectPortal;

    [Header("Sound Settings")]
    [Tooltip("How close the player must be to hear this portal's sound.")]
    public float soundRadius = 4f;

    private AudioSource _audioSource;
    private PortalManager _manager;
    private Transform _player;   // we use player distance for sound

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource != null)
        {
            _audioSource.playOnAwake = false;
            _audioSource.loop = true;
            _audioSource.spatialBlend = 0f; // 2D sound
        }

        // make sure collider is trigger
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    // Called from PortalManager after creating the portal
    public void Setup(PortalManager manager, bool isCorrect, AudioClip clip, Transform playerTransform)
    {
        _manager = manager;
        isCorrectPortal = isCorrect;
        _player = playerTransform;

        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        if (_audioSource != null)
        {
            _audioSource.clip = clip;
            // don't play here; Update will start/stop it depending on distance
        }
    }

    private void Update()
    {
        if (_audioSource == null || _player == null || _audioSource.clip == null)
            return;

        float distance = Vector2.Distance(_player.position, transform.position);

        // Inside radius → play
        if (distance <= soundRadius)
        {
            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }
        // Outside radius → stop
        else
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player OR NPC entering the portal
        if (!other.CompareTag("Player") && !other.CompareTag("NPC"))
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
