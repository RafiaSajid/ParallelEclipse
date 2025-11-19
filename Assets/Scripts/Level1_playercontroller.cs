using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ClearSky
{
    public class Level1_playercontroller : MonoBehaviour
    {
        [Header("Movement")]
        public float movePower = 10f;
        public float jumpPower = 8f; //Set Gravity Scale in Rigidbody2D Component to 5

        [Header("Intro Settings")]
        [Tooltip("How long to wait after the 'die' animation before playing 'idle' and enabling controls.")]
        public float introDelay = 5f; // Customize this delay

        private Rigidbody2D rb;
        private Animator anim;
        Vector3 movement;
        private int direction = 1;
        bool isJumping = false;

        // Start as 'false' so the player can't move during the intro
        private bool alive = false;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();

            // Start the intro sequence
            StartCoroutine(PlayIntroSequence());
        }

        // This Coroutine handles the animation sequence
        IEnumerator PlayIntroSequence()
        {
            // 1. Play the 'die' animation first
            anim.SetTrigger("die");

            // 2. Wait for the specified delay
            yield return new WaitForSeconds(introDelay);

            // 3. Play the 'idle' animation
            anim.SetTrigger("idle");

            // 4. Enable player controls
            alive = true;
        }

        private void Update()
        {
            // Only run if the intro sequence is finished
            if (alive)
            {

                Jump();
                Run();


            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            // This is good, it resets the jump animation
            anim.SetBool("isJump", false);
        }

        // --- IDLE FUNCTION REMOVED ---
        // Calling anim.SetTrigger("idle") every frame in Update()
        // will break your other animations. Let the Animator
        // controller handle returning to Idle automatically.

        void Run()
        {
            Vector3 moveVelocity = Vector3.zero;

            // This is perfect, it resets the 'isRun' bool
            anim.SetBool("isRun", false);


            if (Input.GetAxis("Horizontal") < 0)
            {
                direction = -1;
                moveVelocity = Vector3.left;

                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);

            }
            if (Input.GetAxis("Horizontal") > 0)
            {
                direction = 1;
                moveVelocity = Vector3.right;

                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);

            }
            transform.position += moveVelocity * movePower * Time.deltaTime;
        }
        void Jump()
        {
            if ((Input.GetButtonDown("Jump") || Input.GetAxis("Vertical") > 0)
            && !anim.GetBool("isJump"))
            {
                isJumping = true;
                anim.SetBool("isJump", true);
            }
            if (!isJumping)
            {
                return;
            }

            rb.velocity = Vector2.zero;

            Vector2 jumpVelocity = new Vector2(0, jumpPower);
            rb.AddForce(jumpVelocity, ForceMode2D.Impulse);

            isJumping = false;
        }
    }
}