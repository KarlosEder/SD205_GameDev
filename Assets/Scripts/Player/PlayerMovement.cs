using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement")]
    public float speed = 10f; 
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float sprintMultiplier = 1.5f;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchSpeedMultiplier = 0.5f;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Checks")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool isCrouching;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        HandleCrouch();

        // Speed reset
        float moveSpeed;

        if (isCrouching)
        {
            moveSpeed = speed * crouchSpeedMultiplier;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = speed * sprintMultiplier;
        }
        else
        {
            // Reset speed
            moveSpeed = speed;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Jump if grounded
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        bool crouchPressed = Input.GetKey(crouchKey);
        bool obstacleAbove = Physics.Raycast(transform.position, Vector3.up, standingHeight);

        if (crouchPressed)
        {
            SetCrouchState(true);
        }
        else if (!obstacleAbove)
        {
            SetCrouchState(false);
        }
    }

    void SetCrouchState(bool state)
    {
        isCrouching = state;

        // Set the height based on the state
        controller.height = state ? crouchHeight : standingHeight;

        controller.center = new Vector3(0, controller.height / 2f, 0);
    }
}
