using System.Collections;
using UnityEngine;
using ClearSky;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private bool moveLeft = true;

    [Header("Combat")]
    [SerializeField] private int hitsToDie = 3;            // number of hits before dying
    [SerializeField] private float attackInterval = 1.0f;  // seconds between attacks while colliding
    [SerializeField] private int damagePerAttack = 1;      // damage dealt to player each attack

    [Header("Death")]
    [SerializeField] private float timeBeforeDestroy = 1.2f;

    // internal state
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private int currentHits = 0;
    private bool isAttacking = false;
    private bool isDead = false;
    private Coroutine attackCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // If using physics-based movement, set Rigidbody2D to Kinematic or Dynamic as desired.
        // We'll use transform movement for simplicity; set rb.bodyType = RigidbodyType2D.Kinematic if you want no physics forces.
    }

    private void Start()
    {
        PlayWalk();
    }

    private void Update()
    {
        if (isDead || isAttacking) return;

        // Simple translation movement
        Vector3 dir = moveLeft ? Vector3.left : Vector3.right;
        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    private void PlayWalk()
    {
        animator.ResetTrigger("AttackTrigger");
        animator.ResetTrigger("StunedTrigger");
        animator.ResetTrigger("DeathTrigger");

        animator.SetTrigger("MoveTrigger");

    }

    private void PlayAttack()
    {
        //animator.SetBool("Walk", false);
        animator.SetTrigger("AttackTrigger");
    }

    private void PlayHurt()
    {
        animator.SetTrigger("StunedTrigger");
    }

    private void PlayDie()
    {
        //animator.SetBool("Walk", false);
        animator.SetTrigger("DeathTrigger");
    }

    // Using collision enter/exit (non-trigger). Use OnTriggerEnter2D if you prefer triggers.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Start attacking the player
            if (!isAttacking)
            {
                isAttacking = true;
                attackCoroutine = StartCoroutine(AttackRoutine(collision.gameObject));
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Stop attacking, return to walk
            isAttacking = false;
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            PlayWalk();
        }
    }

    private IEnumerator AttackRoutine(GameObject player)
    {
        // While still colliding and not dead, perform periodic attacks
        while (!isDead && isAttacking)
        {
            PlayAttack();

            // // Try to damage player if it has PlayerHealth
            var ph = player.GetComponent<playerController>();
            if (ph != null)
            {
                ph.TakeDamage(damagePerAttack);
            }


            // wait attack interval
            yield return new WaitForSeconds(attackInterval);

            if (!isDead && isAttacking)
            {
                PlayWalk(); // optionally return to walk between attacks
            }
        }
    }

    // Called by player attack or bullet when this enemy is hit
    public void ReceiveHit(int amount = 1)
    {
        if (isDead) return;

        currentHits += amount;

        if (currentHits < hitsToDie)
        {
            PlayHurt();
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        isAttacking = false;
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);

        // play die animation and disable interaction
        PlayDie();

        // disable collider so it no longer collides with player/bullets
        if (col != null) col.enabled = false;
        // stop movement
        // if using Rigidbody2D velocity: 
        rb.velocity = Vector2.zero;

        Destroy(gameObject, timeBeforeDestroy);
    }

    // Optional helper to flip direction if you want enemies to turn around when hitting boundaries
    public void FlipDirection()
    {
        moveLeft = !moveLeft;
        Vector3 s = transform.localScale;
        s.x = -s.x;
        transform.localScale = s;
    }
}