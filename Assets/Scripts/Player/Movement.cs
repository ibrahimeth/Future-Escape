using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpStrength = 10f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayerMask = 1;
    
    [Header("Physics")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    
    [Header("Input")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private bool jumpInput;
    
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isGrounded;
    private bool wasGrounded;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        
        // Ensure we have required components
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }
        
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
        }
    }

    void Update()
    {
        CheckGrounded();
        HandleCoyoteTime();
        HandleJumpBuffer();
        HandleMovement();
        HandleJumping();
        HandleGravity();
    }
    
    void CheckGrounded()
    {
        wasGrounded = isGrounded;
        
        // Raycast downward from the bottom of the collider
        Vector2 raycastOrigin = new Vector2(col.bounds.center.x, col.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, groundCheckDistance, groundLayerMask);
        
        isGrounded = hit.collider != null;
        
        // Debug visualization
        Debug.DrawRay(raycastOrigin, Vector2.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }
    
    void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }
    
    void HandleJumpBuffer()
    {
        if (jumpInput)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }
    
    void HandleMovement()
    {
        // Horizontal movement
        float targetVelocityX = moveInput.x * speed;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
        
        // Flip sprite based on movement direction
        if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    
    void HandleJumping()
    {
        // Jump if we have jump input buffered and we can jump (grounded or coyote time)
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f; // Consume the jump buffer
        }
    }
    
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
        coyoteTimeCounter = 0f; // Reset coyote time after jumping
    }
    
    void HandleGravity()
    {
        // Apply enhanced gravity for better jump feel
        if (rb.linearVelocity.y < 0)
        {
            // Falling - apply fall multiplier
            rb.linearVelocity += Vector2.up * gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !jumpInput)
        {
            // Rising but not holding jump - apply low jump multiplier
            rb.linearVelocity += Vector2.up * gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    
    // Input System callbacks
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    public void OnJump(InputValue value)
    {
        jumpInput = value.isPressed;
    }
    
    // Public methods for other scripts to use
    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    public Vector2 GetVelocity()
    {
        return rb.linearVelocity;
    }
    
    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }
    
    public void AddForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        rb.AddForce(force, forceMode);
    }
}
