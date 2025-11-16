using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ClearSky
{
    public class Level3_PlayerController : MonoBehaviour
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