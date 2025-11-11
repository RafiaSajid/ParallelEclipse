using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ClearSky
{
    public class playerController : MonoBehaviour
    {
        public float movePower = 10f;
        public float jumpPower = 10f;
        
        // --- Health Variables ---
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 3;
        private int currentHealth;
        private float delayBeforeReload = 2f;
        // ------------------------

        private Rigidbody2D rb;
        private Animator anim;
        Vector3 movement;
        private int direction = 1;
        bool isJumping = false;
        private bool alive = true;

        // Animator hash IDs for performance (good practice)
        private readonly int AnimIsRun = Animator.StringToHash("isRun");
        private readonly int AnimIsJump = Animator.StringToHash("isJump");
        private readonly int AnimAttack = Animator.StringToHash("attack");
        private readonly int AnimHurt = Animator.StringToHash("hurt");
        private readonly int AnimDie = Animator.StringToHash("die");
        private readonly int AnimIdle = Animator.StringToHash("idle");


        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            
            // Initialize health
            currentHealth = maxHealth;
        }

        private void Update()
        {
            Restart();
            if (alive)
            {
                // Hurt() and Die() are now triggered by TakeDamage, so they are removed from Update
                Attack();
                Jump();
                Run();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Reset jump state when touching ground/platform
            anim.SetBool(AnimIsJump, false);
        }

        /// <summary>
        /// Public method for enemies to call when they hit the player.
        /// </summary>
        /// <param name="damage">The amount of health to lose.</param>
        public void TakeDamage(int damage)
        {
            if (!alive) return; // Player is already dead

            currentHealth -= damage;
            
            if (currentHealth <= 0)
            {
                currentHealth = 0; // Prevent health from going into large negatives
                Die();
            }
            else
            {
                Hurt();
            }
        }
        
        // This is called by TakeDamage when health drops below 1
        private void Die()
        {
            alive = false;
            // Stop movement immediately
            rb.velocity = Vector2.zero;
            
            // Play the die animation
            anim.SetTrigger(AnimDie);
            StartCoroutine(LoadGameOverSceneWithDelay(delayBeforeReload));
        }

        private IEnumerator LoadGameOverSceneWithDelay(float delay)
        {
            // Wait for the duration of the death animation/delay
            yield return new WaitForSeconds(delay);
            
            // Load the specified Game Over scene
            SceneManager.LoadScene("School of spirits");
        }

        // This is called by TakeDamage when health is still above 0
        private void Hurt()
        {
            // Play hurt animation
            anim.SetTrigger(AnimHurt);
            
            // Add a knockback force
            float knockbackForce = 5f;
            float knockbackHeight = 1f;
            
            if (direction == 1) // Facing right, knockback left
                rb.AddForce(new Vector2(-knockbackForce, knockbackHeight), ForceMode2D.Impulse);
            else // Facing left, knockback right
                rb.AddForce(new Vector2(knockbackForce, knockbackHeight), ForceMode2D.Impulse);
        }
        
        // --- Movement and Action Methods ---

        void Run()
        {
            Vector3 moveVelocity = Vector3.zero;
            anim.SetBool(AnimIsRun, false);

            float horizontalInput = Input.GetAxisRaw("Horizontal");

            if (horizontalInput < 0)
            {
                direction = -1;
                moveVelocity = Vector3.left;
                
                // Flip the sprite
                transform.localScale = new Vector3(direction, 1, 1);
                
                if (!anim.GetBool(AnimIsJump))
                    anim.SetBool(AnimIsRun, true);
            }
            else if (horizontalInput > 0)
            {
                direction = 1;
                moveVelocity = Vector3.right;

                // Flip the sprite
                transform.localScale = new Vector3(direction, 1, 1);
                
                if (!anim.GetBool(AnimIsJump))
                    anim.SetBool(AnimIsRun, true);
            }
            
            // Apply movement
            transform.position += moveVelocity * movePower * Time.deltaTime;
        }

        void Jump()
        {
            if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
            && !anim.GetBool(AnimIsJump))
            {
                isJumping = true;
                anim.SetBool(AnimIsJump, true);
            }
            
            if (!isJumping)
            {
                return;
            }

            // A small hack to reset velocity before applying impulse for clean jumps
            rb.velocity = new Vector2(rb.velocity.x, 0); 
            
            Vector2 jumpVelocity = new Vector2(0, jumpPower);
            rb.AddForce(jumpVelocity, ForceMode2D.Impulse);

            isJumping = false;
        }

        void Attack()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                anim.SetTrigger(AnimAttack);
                // NOTE: Add your attack/hitbox logic here
            }
        }
        
        void Restart()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0) && !alive)
            {
                // Reset to full health and active state
                currentHealth = maxHealth;
                alive = true;
                
                anim.SetTrigger(AnimIdle);
                // Optionally re-enable collider
                // GetComponent<Collider2D>().enabled = true;
            }
        }
    }
}