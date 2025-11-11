using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class npcFollow : MonoBehaviour
{
[Header("References")]
    public Transform player;           // Reference to main player
    public Animator npcAnimator;       // NPC Animator
    private NavMeshAgent agent;        // NavMesh agent for pathfinding

    [Header("Settings")]
    public float followDistance = 2f;   // Minimum distance to maintain from player
    public float followRange = 10f;     // Distance within which NPC follows
    public float jumpFollowDelay = 0.2f; // Delay before NPC mimics jump
    public float stopAttackTime = 1f;   // How long NPC stays idle after attack

    private Animator playerAnimator;    // Reference to player animator
    private bool isFollowing = false;
    private bool isPerformingAction = false; // To handle attack/hurt pauses
    private bool hasDied = false;  
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player != null)
            playerAnimator = player.GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null || npcAnimator == null || agent == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Determine if within follow range
        isFollowing = distance <= followRange;

        // If NPC is following and not doing an action (attack/hurt)
        if (isFollowing && !isPerformingAction)
        {
            FollowPlayer(distance);
        }
        else if (!isFollowing)
        {
            agent.isStopped = true;
            npcAnimator.SetBool("isRun", false);
        }

        // Continuously sync animations with player
        SyncAnimations();
    }

    void FollowPlayer(float distance)
    {
        if (distance > followDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            npcAnimator.SetBool("isRun", true);
        }
        else
        {
            agent.isStopped = true;
            npcAnimator.SetBool("isRun", false);
        }
    }

    void SyncAnimations()
    {
        if (playerAnimator == null)
            return;

        // Read player's animation states
        bool playerIsRun = playerAnimator.GetBool("isRun");
        bool playerIsJump = playerAnimator.GetBool("isJump");
        bool playerIsAttack = playerAnimator.GetBool("attack");
        bool playerIsHurt = playerAnimator.GetBool("hurt");

        AnimatorStateInfo playerState = playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (playerState.IsName("Die") && !hasDied)
        {
            npcAnimator.SetTrigger("die");
            hasDied = true;
            agent.isStopped = true;
            return;
        }

        // --- Handle attack ---
        if (playerIsAttack && !isPerformingAction)
        {
            isPerformingAction = true;
            agent.isStopped = true; // stop moving during attack
            npcAnimator.SetTrigger("attack");
            Invoke(nameof(ResumeFollowing), stopAttackTime);
            return; // stop syncing other actions temporarily
        }

        // --- Handle hurt ---
        if (playerIsHurt && !isPerformingAction)
        {
            isPerformingAction = true;
            agent.isStopped = true;
            npcAnimator.SetTrigger("hurt");
            Invoke(nameof(ResumeFollowing), stopAttackTime);
            return;
        }

        // --- Handle jump ---
        if (playerIsJump)
        {
            Invoke(nameof(PerformJump), jumpFollowDelay);
        }

        // --- Handle run/idle ---
        npcAnimator.SetBool("isRun", playerIsRun);
    }

    void PerformJump()
    {
        npcAnimator.SetTrigger("isJump");
    }

    void ResumeFollowing()
    {
        isPerformingAction = false;
        if(!hasDied)
            agent.isStopped = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followDistance);
    }
}

//    [Header("Target Settings")]
//     public Transform player;              // Player reference

//     [Header("Movement Settings")]
//     public float followSpeed = 3f;        // Speed when following
//     public float rotationSpeed = 5f;      // Turning smoothness
//     public float followDistance = 2f;     // Distance to maintain from player
//     public float followRange = 10f;       // Start following only if player is within this range

//     [Header("Animation Settings")]
//     public Animator anim;                 // Animator controlling the NPC

//     private bool isFollowing = false;

//     void Update()
//     {
//         if (player == null) return;

//         float distance = Vector3.Distance(transform.position, player.position);

//         // Check if player is within follow range
//         if (distance <= followRange)
//         {
//             isFollowing = true;
//         }
//         else
//         {
//             isFollowing = false;
//         }

//         // Handle following behavior
//         if (isFollowing)
//         {
//             // Move closer if too far from player
//             if (distance > followDistance)
//             {
//                 Vector3 direction = (player.position - transform.position).normalized;
//                 Vector3 targetPosition = player.position - direction * followDistance;

//                 transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);

//                 // Set walking animation
//                 if (anim != null)
//                     anim.SetBool("isRun", true);
//             }
//             else
//             {
//                 // Stop moving when close enough
//                 if (anim != null)
//                     anim.SetBool("isRun", false);
//             }

//             // Smoothly face the player
//             Vector3 lookDirection = (player.position - transform.position).normalized;
//             lookDirection.y = 0; // keep upright
//             if (lookDirection != Vector3.zero)
//             {
//                 Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
//                 transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
//             }
//         }
//         else
//         {
//             // Stop animation when player too far
//             if (anim != null)
//                 anim.SetBool("isRun", false);
//         }
//     }

//     void OnDrawGizmosSelected()
//     {
//         // Draw follow range in Scene view for debugging
//         Gizmos.color = Color.green;
//         Gizmos.DrawWireSphere(transform.position, followRange);

//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position, followDistance);
//     }
// }

//     public Transform target;
//     public float moveSpeed = 9f;
//     public Animator anim;
//     private bool canFollow = false;
//     // Start is called before the first frame update
//     void Start()
//     {
//         anim = GetComponent<Animator>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if(canFollow || target == null) return;

//         Vector2 direction = (target.position - transform.position).normalized;
//         transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

//         anim.SetFloat("magnitude", direction.magnitude);

//     }

//     public void StartFollowing()
//     {
//         canFollow = true;
//     }
// }
//trigger
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class npcTrigger : MonoBehaviour
// {   
//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Player"))
//         {
//             GameObject npc = GameObject.FindGameObjectWithTag("NPC");

                
//                     npcFollow followScript = npc.GetComponent<npcFollow>();

//                     if (followScript != null)
//                     {
//                         followScript.StartFollowing();
//                     }
               
//         } Destroy(gameObject);
//     }
//     // Start is called before the first frame update
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }
// }

