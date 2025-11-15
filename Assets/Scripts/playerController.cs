using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ClearSky
{
    public class playerController : MonoBehaviour
    {
        [Header("Movement")]
        public float movePower = 10f;
        public float jumpPower = 8f;

        [Header("Health Settings")]
        private int maxHealth = 100;
        private int currentHealth;
        private float delayBeforeReload = 2f; // reload the same scene after death

        private Rigidbody2D rb;
        private Animator anim;

        private int direction = 1;
        private bool isJumping = false;
        private bool alive = true;
        [SerializeField] private Collider2D attackHitbox;
        [Header("Events")]
        [SerializeField] private GameEvent OnHealthChanged;
        [SerializeField] private GameEvent OnPlayerDied;
        [SerializeField] private GameEvent OnHeartPickup;

        private Vector3 baseScale;
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;


        // Animator IDs
        private readonly int AnimIsRun = Animator.StringToHash("isRun");
        private readonly int AnimIsJump = Animator.StringToHash("isJump");
        private readonly int AnimAttack = Animator.StringToHash("attack");
        private readonly int AnimHurt = Animator.StringToHash("hurt");
        private readonly int AnimDie = Animator.StringToHash("die");

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            baseScale = transform.localScale;

            currentHealth = maxHealth;
        }

        void Update()
        {
            if (!alive) return;

            Run();
            Attack();
            Jump();
        }

        // ---------------- Movement ----------------

        void Run()
        {
            float h = Input.GetAxisRaw("Horizontal");

            // Apply velocity
            rb.velocity = new Vector2(h * movePower, rb.velocity.y);

            // Set animation
            anim.SetBool(AnimIsRun, h != 0 && !anim.GetBool(AnimIsJump));

            // Flip
            if (h < 0)
            {
                direction = -1;
                transform.localScale = new Vector3(baseScale.x * -1, baseScale.y, baseScale.z);
            }
            else if (h > 0)
            {
                direction = 1;
                transform.localScale = new Vector3(baseScale.x * 1, baseScale.y, baseScale.z);
            }
        }

        void Jump()
        {
            if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0) &&
                !anim.GetBool(AnimIsJump))
            {
                isJumping = true;
                anim.SetBool(AnimIsJump, true);
            }

            if (!isJumping) return;

            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            isJumping = false;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            // This assumes your ground objects use a TRIGGER collider.
            anim.SetBool(AnimIsJump, false);
            if (col.gameObject.CompareTag("zombie"))
            {
                TakeDamage(10); // Example damage value
                Hurt();
            }
        }

        // ---------------- Combat ----------------

        void Attack()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                anim.SetTrigger(AnimAttack);
                StartCoroutine(EnableHitbox());
                StartCoroutine(DisableHitboxAfterDelay(0.3f));
            }
        }

        private IEnumerator EnableHitbox()
        {
            attackHitbox.enabled = true;
            yield return new WaitForSeconds(0.2f); // attack duration
            attackHitbox.enabled = false;
        }

        private IEnumerator DisableHitboxAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            attackHitbox.enabled = false;
        }


        // ---------------- Damage / Death ----------------

        public void TakeDamage(int dmg)
        {
            if (!alive) return;

            currentHealth -= dmg;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            OnHealthChanged.Raise();

            if (currentHealth <= 0)
            {
                alive = false;
                OnPlayerDied.Raise();
                Die();
            }
            else
            {
                Hurt();
            }
        }

        public void AddHealth(int amount)
        {
            if (!alive) return;

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            Debug.Log("Health increased. Current Health = " + currentHealth);
            OnHeartPickup.Raise();
            OnHealthChanged.Raise();
        }



        public void Hurt()
        {
            anim.SetTrigger(AnimHurt);

            // float force = 5f;
            // float vertical = 1f;

            // rb.AddForce(new Vector2(-direction * force, vertical), ForceMode2D.Impulse);
        }


        public void Die()
        {
            alive = false;
            rb.velocity = Vector2.zero;

            anim.SetTrigger(AnimDie);

            StartCoroutine(RestartSceneAfterDelay(delayBeforeReload));
        }

        private IEnumerator RestartSceneAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Reload the current active scene
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

    }
}






// using UnityEngine;
// using UnityEngine.SceneManagement;
// using System.Collections;

// namespace ClearSky
// {
//     public class playerController : MonoBehaviour
//     {
//         public float movePower = 10f;
//         public float jumpPower = 10f;

//         // --- Health Variables ---
//         [Header("Health Settings")]
//         [SerializeField] private int maxHealth = 100;
//         private int currentHealth;
//         private float delayBeforeReload = 2f;
//         // ------------------------

//         private Rigidbody2D rb;
//         private Animator anim;
//         Vector3 movement;
//         private int direction = 1;
//         bool isJumping = false;
//         private bool alive = true;

//         // Animator hash IDs for performance (good practice)
//         private readonly int AnimIsRun = Animator.StringToHash("isRun");
//         private readonly int AnimIsJump = Animator.StringToHash("isJump");
//         private readonly int AnimAttack = Animator.StringToHash("attack");
//         private readonly int AnimHurt = Animator.StringToHash("hurt");
//         private readonly int AnimDie = Animator.StringToHash("die");
//         private readonly int AnimIdle = Animator.StringToHash("idle");


//         // Start is called before the first frame update
//         void Start()
//         {
//             rb = GetComponent<Rigidbody2D>();
//             anim = GetComponent<Animator>();

//             // Initialize health
//             currentHealth = maxHealth;
//         }

//         private void Update()
//         {
//             Restart();
//             if (alive)
//             {
//                 // Hurt() and Die() are now triggered by TakeDamage, so they are removed from Update
//                 Attack();
//                 Jump();
//                 Run();
//             }
//         }

//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             // Reset jump state when touching ground/platform
//             anim.SetBool(AnimIsJump, false);
//         }

//         /// <summary>
//         /// Public method for enemies to call when they hit the player.
//         /// </summary>
//         /// <param name="damage">The amount of health to lose.</param>
//         public void TakeDamage(int damage)
//         {
//             if (!alive) return; // Player is already dead

//             currentHealth -= damage;

//             if (currentHealth <= 0)
//             {
//                 currentHealth = 0; // Prevent health from going into large negatives
//                 Die();
//             }
//             else
//             {
//                 Hurt();
//             }
//         }

//         // This is called by TakeDamage when health drops below 1
//         private void Die()
//         {
//             alive = false;
//             // Stop movement immediately
//             rb.velocity = Vector2.zero;

//             // Play the die animation
//             anim.SetTrigger(AnimDie);
//             StartCoroutine(LoadGameOverSceneWithDelay(delayBeforeReload));
//         }

//         private IEnumerator LoadGameOverSceneWithDelay(float delay)
//         {
//             // Wait for the duration of the death animation/delay
//             yield return new WaitForSeconds(delay);

//             // Load the specified Game Over scene
//             SceneManager.LoadScene("Post-apocalyptic");
//         }

//         // This is called by TakeDamage when health is still above 0
//         private void Hurt()
//         {
//             // Play hurt animation
//             anim.SetTrigger(AnimHurt);

//             // Add a knockback force
//             float knockbackForce = 5f;
//             float knockbackHeight = 1f;

//             if (direction == 1) // Facing right, knockback left
//                 rb.AddForce(new Vector2(-knockbackForce, knockbackHeight), ForceMode2D.Impulse);
//             else // Facing left, knockback right
//                 rb.AddForce(new Vector2(knockbackForce, knockbackHeight), ForceMode2D.Impulse);
//         }

//         // --- Movement and Action Methods ---

//         void Run()
//         {
//             Vector3 moveVelocity = Vector3.zero;
//             anim.SetBool(AnimIsRun, false);

//             float horizontalInput = Input.GetAxisRaw("Horizontal");

//             if (horizontalInput < 0)
//             {
//                 direction = -1;
//                 moveVelocity = Vector3.left;

//                 // Flip the sprite
//                 transform.localScale = new Vector3(direction, 1, 1);

//                 if (!anim.GetBool(AnimIsJump))
//                     anim.SetBool(AnimIsRun, true);
//             }
//             else if (horizontalInput > 0)
//             {
//                 direction = 1;
//                 moveVelocity = Vector3.right;

//                 // Flip the sprite
//                 transform.localScale = new Vector3(direction, 1, 1);

//                 if (!anim.GetBool(AnimIsJump))
//                     anim.SetBool(AnimIsRun, true);
//             }

//             // Apply movement
//             transform.position += moveVelocity * movePower * Time.deltaTime;
//         }

//         void Jump()
//         {
//             if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
//             && !anim.GetBool(AnimIsJump))
//             {
//                 isJumping = true;
//                 anim.SetBool(AnimIsJump, true);
//             }

//             if (!isJumping)
//             {
//                 return;
//             }

//             // A small hack to reset velocity before applying impulse for clean jumps
//             rb.velocity = new Vector2(rb.velocity.x, 0);

//             Vector2 jumpVelocity = new Vector2(0, jumpPower);
//             rb.AddForce(jumpVelocity, ForceMode2D.Impulse);

//             isJumping = false;
//         }

//         void Attack()
//         {
//             if (Input.GetKeyDown(KeyCode.Return))
//             {
//                 anim.SetTrigger(AnimAttack);
//                 // NOTE: Add your attack/hitbox logic here
//             }
//         }

//         void Restart()
//         {
//             if (Input.GetKeyDown(KeyCode.Alpha0) && !alive)
//             {
//                 // Reset to full health and active state
//                 currentHealth = maxHealth;
//                 alive = true;

//                 anim.SetTrigger(AnimIdle);
//                 // Optionally re-enable collider
//                 // GetComponent<Collider2D>().enabled = true;
//             }
//         }
//     }
// }