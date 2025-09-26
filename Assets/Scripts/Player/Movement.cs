using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float jumpStrength = 5;
    [SerializeField] private Camera myCamera;
    [SerializeField] private AudioSource walkingAudio;
    [SerializeField] private AudioSource jumpAudio;
    [SerializeField] private float obstacleRaycastDistance = 1f;
    [SerializeField] private float doubleJumpCooldown = 0.2f;

    private Rigidbody rb;
    private Vector2 movement;
    private bool canDoubleJump = true;

    private bool IsGrounded
    {
        get
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
            {
                return hit.collider.CompareTag("Ground");
            }
            return false;
        }
    }

    private bool IsHittingObstacle
    {
        get
        {
            // Get the movement direction
            Vector3 rayDirection = new Vector3(movement.x, 0, 0).normalized;
            
            // Only check if we're actually moving
            if (rayDirection.magnitude < 0.1f) return false;
            
            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, obstacleRaycastDistance))
            {
                Debug.DrawRay(transform.position, rayDirection * obstacleRaycastDistance, Color.red);
                // Check if we hit an obstacle
                return hit.collider.CompareTag("Obstacle");
            }
            return false;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void Update()
    {
        Move();
        doubleJumpCooldownUpdate();
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
            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);

            // Reset double jump if grounded or hitting obstacle
            if ((IsGrounded || IsHittingObstacle) && doubleJumpCooldown <= 0)
            {
                canDoubleJump = true;
                doubleJumpCooldown = 0.2f; // Reset cooldown
            }
            else if (!IsGrounded)
            {
                canDoubleJump = false;
            }

            // Play jump audio
            if (jumpAudio != null)
            {
                jumpAudio.Play();
            }
        }
    }

    private void doubleJumpCooldownUpdate()
    {
        if (doubleJumpCooldown >= 0)
        {
            doubleJumpCooldown -= Time.deltaTime;
        }
    }

    private void HandleWalkingAudio(Vector3 moveDirection)
    {
        if (walkingAudio == null) return;

        // Check if player is moving
        bool isMoving = moveDirection.magnitude > 0.1f;

        if (isMoving && IsGrounded)
        {
            // Start playing walking audio if not already playing
            if (!walkingAudio.isPlaying)
            {
                walkingAudio.Play();
            }
        }
        else
        {
            // Stop walking audio when not moving
            if (walkingAudio.isPlaying)
            {
                walkingAudio.Stop();
            }
        }
    }
}