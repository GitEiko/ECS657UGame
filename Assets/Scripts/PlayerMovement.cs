using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction sprintAction;

    [SerializeField] float speed = 5f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float lookSensitivity = 1f;
    [SerializeField] Transform playerCamera;
    float verticalVelocity = 0f;
    [SerializeField] float gravity = -9.81f;

    float xRotation = 0f;
    bool isSprinting = false;

    CharacterController characterController;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();

        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
        sprintAction = playerInput.actions.FindAction("Sprint");

        Cursor.lockState = CursorLockMode.Locked;

        sprintAction.performed += context => StartSprinting();
        sprintAction.canceled += context => StopSprinting();
    }

    void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = -0.5f; 
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }


    void Update()
    {
        ApplyGravity();
        MovePlayer();
        LookAround();
    }

    void MovePlayer()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        float currentSpeed = isSprinting ? sprintSpeed : speed;

        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;

        moveDirection.y = verticalVelocity;

        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
    }


    void LookAround()
    {
        Vector2 lookVector = lookAction.ReadValue<Vector2>();

        transform.Rotate(Vector3.up * lookVector.x * lookSensitivity);

        xRotation -= lookVector.y * lookSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void StartSprinting()
    {
        isSprinting = true;
    }

    void StopSprinting()
    {
        isSprinting = false;
    }
}
