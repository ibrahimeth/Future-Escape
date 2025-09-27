using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float jumpStrength = 5;
    [SerializeField] private float doubleJumpCooldown = 0.1f;
    [SerializeField] private Camera myCamera;

    [Header("Wall Sliding Settings")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallSlideSpeed = 2f;
    private bool isWallTouching;
    private bool isWallSliding;

    private Animator animator;
    private bool isRunning = false;
    private bool isFalling = false;
    private bool isJumping = false;

    private Rigidbody rb;
    private Vector2 movement;
    private bool canDoubleJump = true;
    private bool isFacingRight = true;
    private bool shouldFreezeControls;

    private bool IsGrounded
    {
        get
        {
            if (groundCheck != null)
            {
                return Physics.OverlapBox(groundCheck.position, new Vector3(0.3f, 0.1f, 0.5f), Quaternion.identity, groundLayer).Length > 0;
            }
            else
            {
                Debug.LogWarning("GroundCheck transform is not assigned.");
            }
            return false;
        }
    }

    private bool IsWallTouching
    {
        get
        {
            if (wallCheck != null)
            {
                return Physics.OverlapBox(wallCheck.position, new Vector3(0.14f, 0.5f, 0.5f), Quaternion.identity, groundLayer).Length > 0;
            }
            else
            {
                Debug.LogWarning("WallCheck transform is not assigned.");
            }
            return false;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (shouldFreezeControls) return;

        HandleFallingAnimation();
        HandleFallDeath();
    }

    void FixedUpdate()
    {
        if (shouldFreezeControls) return;

        Move();
        UpdateFacingDirection();
        HandleRunningAnimation();
        CheckWallSliding();
        HandleWallSliding();

        // Reset falling and jumping states when player lands
        if (IsGrounded && (isFalling || isJumping))
        {
            isFalling = false;
            isJumping = false;
        }
    }

    public void OnJump(InputValue value)
    {
        if (shouldFreezeControls) return;

        Jump(value);
    }

    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    private void Move()
    {
        Vector3 moveDirection = new Vector3(movement.x, 0, 0);
        Vector3 worldMoveDirection = transform.TransformDirection(moveDirection);

        // Duvara temas ettiğinde o yöne hareket etmeyi engelle
        if (IsWallTouching)
        {
            Debug.Log("Wall touching, checking movement direction.");
            // Eğer oyuncu face ettiği yöne (duvara doğru) hareket etmeye çalışıyorsa, bunu engelle
            if ((isFacingRight && movement.x > 0) || (!isFacingRight && movement.x < 0))
            {
                // Duvara doğru hareket etme, sadece ters yöne gitmesine izin ver
                return;
            }
        }

        transform.position += worldMoveDirection * Time.deltaTime * moveSpeed;
    }

    private void Jump(InputValue value)
    {
        // Normal jump ve double jump
        if (value.isPressed && (IsGrounded || canDoubleJump))
        {
            if (!IsGrounded)
            {
                canDoubleJump = false;
            }
            else
            {
                canDoubleJump = true;
            }
            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            isJumping = true; // Set jumping state when jump is initiated
        }
    }

    private void HandleRunningAnimation()
    {
        if (animator == null || !IsGrounded) return;

        if (movement.x != 0 && !isRunning)
        {
            isRunning = true;
            animator.SetBool("Is Running", true);
        }
        else if (movement.x == 0 && isRunning)
        {
            isRunning = false;
            animator.SetBool("Is Running", false);
        }
    }

    private void HandleFallingAnimation()
    {
        if (animator == null) return;

        // Wall sliding animasyonu
        if (isWallSliding && !isFalling)
        {
            animator.Play("Fall State"); // Wall slide için özel animasyon varsa "Wall Slide State" kullanın
            return;
        }

        if (!IsGrounded && !isFalling && !isWallSliding)
        {
            animator.Play("Fall State");
            isFalling = true;
        }
        if (IsGrounded)
        {
            if (movement.x == 0)
            {
                animator.Play("Idle State");

            }
            else
            {
                animator.Play("Run State");
            }
            isFalling = false;
        }
    }

    private void HandleFallDeath()
    {
        if (transform.position.y < -10)
        {
            shouldFreezeControls = true;
            animator.Play("Fall Death State");
        }
    }

    private void UpdateFacingDirection()
    {
        if (movement.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (movement.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void CheckWallSliding()
    {
        // Wall sliding kontrolü
        if (IsWallTouching && !IsGrounded && rb.linearVelocity.y < 0)
        {
            // Oyuncu duvara doğru hareket ediyorsa wall slide yap
            if ((isFacingRight && movement.x > 0) || (!isFacingRight && movement.x < 0))
            {
                isWallSliding = true;
            }
            else
            {
                isWallSliding = false;
            }
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void HandleWallSliding()
    {
        if (isWallSliding)
        {
            // Düşme hızını sınırla (wall slide speed)
            if (rb.linearVelocity.y < -wallSlideSpeed)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallSlideSpeed, rb.linearVelocity.z);
            }

            // Double jump'ı tekrar aktif et
            canDoubleJump = true;
        }
    }
}