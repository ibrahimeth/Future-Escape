using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float jumpStrength = 5;
    [SerializeField] private float jumbStartLimit = 0.01f;
    [SerializeField] private Camera myCamera;
    [SerializeField] private AudioSource walkingAudio;
    [SerializeField] private AudioSource jumpAudio;

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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void Update()
    {
        Move();
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
            if (!IsGrounded)
            {
                canDoubleJump = false;
            }
            else
            {
                canDoubleJump = true;
            }

            // Play jump audio
            if (jumpAudio != null)
            {
                jumpAudio.Play();
            }
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