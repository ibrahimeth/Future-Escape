using System;
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

    [Header("Wall Jump System")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float isWallSlidingSpeed = 2;
    private bool isWallTouching;
    private bool isWallSliding;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    private bool isRunning = false;
    private bool isFalling = false;
    private bool isJumping = false;

    private Rigidbody rb;
    private Vector2 movement;
    private bool canDoubleJump = true;
    private bool isFacingRight = true;

    private bool IsGrounded
    {
        get
        {
            if (Physics.OverlapBox(transform.position, new Vector3(0.5f, 1f, 0.5f), Quaternion.identity, LayerMask.GetMask("Ground")).Length > 0)
            {
                return true;
            }
            return false;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void Update()
    {
        Move();
        UpdateFacingDirection();
        HandleRunningAnimation();
        HandleFallingAnimation();

        // Reset falling and jumping states when player lands
        if (IsGrounded && (isFalling || isJumping))
        {
            isFalling = false;
            isJumping = false;
        }
    }

    public void OnJump(InputValue value)
    {
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

        transform.position += worldMoveDirection * Time.deltaTime * moveSpeed;
    }

    private void Jump(InputValue value)
    {
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

        if (!IsGrounded && !isFalling)
        {
            animator.Play("Fall State");
            isFalling = true;
        } if(IsGrounded && isFalling)
        {
            animator.Play("Idle State");
            isFalling = false;
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


}