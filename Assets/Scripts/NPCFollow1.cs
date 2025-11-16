using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollow : MonoBehaviour
{
    
    [Header("References")]
    public Transform followPlayer;             // Assign the Player transform
    public Rigidbody2D rb;                     // NPC’s Rigidbody2D
    public Animator animator;                  // NPC Animator (unique controller!)

    [Header("Follow Settings")]
    public float speed = 2f;                   // Horizontal move speed
    public float followDistance = 2.5f;        // Min distance to maintain from player
    public float yClampMin = -5f;              // Optional Y clamp for jump
    public float yClampMax = 2f;

    [Header("Jump Settings")]
    public Transform groundCheck;              // Empty object under NPC feet
    public float checkRadius = 0.1f;
    public LayerMask groundLayer;              // Assign to "Ground" layer in Inspector
    public float jumpForce = 6f;
    public float jumpCooldown = 1f;
    private bool isGrounded;
    private float jumpTimer;

    private bool canFlip = true;
    private bool isJumping = false;
    private Vector3 initialScale;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        // Save initial scale to prevent shrinking problems
        initialScale = transform.localScale;

        // Optional: ignore physical collision with player
        if (followPlayer != null)
        {
            Collider2D npcCol = GetComponent<Collider2D>();
            Collider2D playerCol = followPlayer.GetComponent<Collider2D>();
            if (npcCol && playerCol)
                Physics2D.IgnoreCollision(npcCol, playerCol, true);
        }

        // If you use physics-based follow start after first FixedUpdate
        StartCoroutine(InitializeAfterPhysics());
    }

    IEnumerator InitializeAfterPhysics()
    {
        yield return new WaitForFixedUpdate();
        canFlip = true;
        animator.SetTrigger("idle");
    }

    void FixedUpdate()
    {
        if (followPlayer == null) return;

        // === Check ground ===
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // === Horizontal follow logic ===
        float xDiff = followPlayer.position.x - transform.position.x;

        if (Mathf.Abs(xDiff) > followDistance)
        {
            rb.velocity = new Vector2(Mathf.Sign(xDiff) * speed, rb.velocity.y);

            if (canFlip)
                transform.localScale = new Vector3(Mathf.Sign(xDiff) * Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);

            animator.ResetTrigger("idle");
            animator.SetBool("isRun", true);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("isRun", false);
            animator.SetTrigger("idle");
        }

        // === Jump behavior (optional, for platforms or ledges) ===
        jumpTimer -= Time.fixedDeltaTime;
        if (isGrounded && jumpTimer <= 0f)
        {
            float yDiff = followPlayer.position.y - transform.position.y;

            // Jump if player is much higher
            if (yDiff > 1f)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetBool("isJumpUp", true);
                isJumping = true;
                jumpTimer = jumpCooldown;
            }
        }
    }

    void LateUpdate()
    {
        // Keep consistent scale (fixes shrinking bug)
        float dir = Mathf.Sign(transform.localScale.x);
        transform.localScale = new Vector3(dir * Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);

        // If landed from jump, switch to jump-down or idle
        if (isJumping && isGrounded)
        {
            animator.SetBool("isJumpUp", false);
            animator.SetBool("isJumpDown",true);
            isJumping = false;
        }

        // Optional Y clamp to avoid drifting vertically
        float clampedY = Mathf.Clamp(transform.position.y, yClampMin, yClampMax);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    }

    // Just for debugging
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}










// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class NPCFollow : MonoBehaviour
// {
//     public Transform followplayer; // Reference to the player's transform
//     public float distanceFromChar;
//     public List<Vector2> followCharPos = new List<Vector2>();
//     public float allowableDistance;
//     public float sampleTimediff;
//     float sampleTime;
//     public float followspeed;
//     public float fastMarch;
//     public float normalSpeed;
//     public float fastDistance;
//     //private RigidBody2D rb;
//     //private Animator anim;
//     // Start is called before the first frame update
//     void Start()
//     {

//         sampleTime = Time.time;
//         followCharPos.Add(followplayer.position);
//         followspeed = fastMarch;
//         // rb = GetComponent<Rigidbody2D>();
//         //anim = GetComponent<Animator>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (Time.time > sampleTime)
//         {
//             sampleTime = Time.time + sampleTimediff;
//             if (Vector2.Distance(transform.position, followplayer.position) > distanceFromChar
//             && Vector2.Distance(followplayer.position, followCharPos[followCharPos.Count - 1]) > allowableDistance)
//             {
//                 followCharPos.Add(followplayer.position);
//             }

//         }
//         if (Vector2.Distance(transform.position, followplayer.position) > fastDistance)
//         {
//             followspeed = fastMarch;
//         }
//         else
//         {
//             followspeed = normalSpeed;
//         }

//         if (Vector2.Distance(transform.position, followplayer.position) > distanceFromChar)
//         {
//             //Vector2 dir = (followCharPos[0] - (Vector2)transform.position).normalized;
//             transform.position = Vector2.MoveTowards(transform.position, followCharPos[0], Time.deltaTime * followspeed);
//         }
//         if (Vector2.Distance(transform.position, followCharPos[0]) < 0.05f)//0.05 remove dist variable
//         {
//             if (followCharPos.Count > 1) followCharPos.RemoveAt(0);
//         }
//     }
// }
