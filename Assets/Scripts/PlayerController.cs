using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private movementType movementState;
    private enum movementType { IDLE, MOVE_LEFT, MOVE_RIGHT };
    [SerializeField]
    private bool isJumping = false;
    [SerializeField]
    private float jumpForce = 20f;
    [SerializeField]
    private bool isGrounded = false;
    [SerializeField]
    private bool isDead = false;
    private Coroutine lastRoutine;
    [SerializeField]
    private AudioClip[] deathClips;
    private GameManager gameManager;
    private int foreGroundLayer;
    private int playerLayer;
    [SerializeField]
    private Animator animationController;
    [SerializeField]
    private Transform spriteTransform;

    private AudioSource audioSource;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        movementState = movementType.IDLE;
        rb = gameObject.GetComponent<Rigidbody2D>();
        isJumping = false;
        isGrounded = false;
        isDead = false;
        gameManager = FindObjectOfType<GameManager>();
        animationController = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        foreGroundLayer = LayerMask.NameToLayer("Foreground");
        playerLayer = LayerMask.NameToLayer("Player");
        EnablePlayerCollision();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movementState = movementType.MOVE_RIGHT;
            animationController.SetBool("isRunning", true);
            Vector3 scale = spriteTransform.localScale;
            scale.x = 1f;
            spriteTransform.localScale = scale;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            if(movementState == movementType.MOVE_RIGHT)
            {
                movementState = movementType.IDLE;
                animationController.SetBool("isRunning", false);
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
        {
            movementState = movementType.MOVE_LEFT;
            animationController.SetBool("isRunning", true);
            Vector3 scale = spriteTransform.localScale;
            scale.x = -1f;
            spriteTransform.localScale = scale;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.Q))
        {
            if (movementState == movementType.MOVE_LEFT)
            {
                movementState = movementType.IDLE;
                animationController.SetBool("isRunning", false);
            }
        }

        if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.J)) && isGrounded)
        {
            isJumping = true;
        }

        if (!isDead)
        {
            if (movementState != movementType.IDLE)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
                audioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity;
        velocity = rb.velocity;
        velocity.x = 0f;
        if (!isDead)
        {
            if (isJumping)
            {
                velocity.y = jumpForce;
            }
            if (movementState == movementType.MOVE_LEFT)
            {
                velocity.x = -10f;
            }

            if (movementState == movementType.MOVE_RIGHT)
            {
                velocity.x = 10f;
            }
        }
        rb.velocity = velocity;
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(0.08f);
        isGrounded = false;
        isJumping = false;
    }

    IEnumerator Death()
    {
        Vector2 velocity;
        DisablePlayerCollision();
        velocity = rb.velocity;
        velocity.y = 20f;
        rb.velocity = velocity;
        int selection = Random.Range(0, deathClips.Length);
        audioSource.PlayOneShot(deathClips[selection]);
        yield return new WaitForSeconds(1f);
        gameManager.TakeLife();

    }

    private void DisablePlayerCollision()
    {
        int mask = Physics2D.GetLayerCollisionMask(playerLayer);
        mask &= ~(1 << foreGroundLayer);
        Physics2D.SetLayerCollisionMask(playerLayer, mask);

        mask = Physics2D.GetLayerCollisionMask(foreGroundLayer);
        mask &= ~(1 << playerLayer);
        Physics2D.SetLayerCollisionMask(foreGroundLayer, mask);
    }

    private void EnablePlayerCollision()
    {
        int mask = Physics2D.GetLayerCollisionMask(playerLayer);
        mask |= (1 << foreGroundLayer);
        Physics2D.SetLayerCollisionMask(playerLayer, mask);

        mask = Physics2D.GetLayerCollisionMask(foreGroundLayer);
        mask |= (1 << playerLayer);
        Physics2D.SetLayerCollisionMask(foreGroundLayer, mask);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (lastRoutine != null)
                StopCoroutine(lastRoutine);
            isGrounded = true;
        }
        if (!isDead)
        {
            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("RoofSpike"))
            {
                isDead = true;
                audioSource.Stop();
                StartCoroutine(Death());
                Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            
            lastRoutine = StartCoroutine(CoyoteTime());
            isJumping = false;
        }
    }
}
