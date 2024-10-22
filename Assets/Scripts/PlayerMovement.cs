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

    [SerializeField] float speed = 5;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float lookSensitivity = 1f;
    [SerializeField] Transform playerCamera;
    
    float xRotation = 0f;
    bool isSprinting = false;



    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
        sprintAction = playerInput.actions.FindAction("Sprint");
        Cursor.lockState = CursorLockMode.Locked;

        sprintAction.performed += context => StartSprinting();
        sprintAction.canceled += context => StopSprinting();
    }

    void Update()
    {
        MovePlayer();
        LookAround();
    }

    void MovePlayer()
    {
        Vector2 vector2 = moveAction.ReadValue<Vector2>();
        float currentSpeed = isSprinting ? sprintSpeed : speed;
        Vector3 moveVector = transform.right * vector2.x + transform.forward * vector2.y;
        transform.position += moveVector * Time.deltaTime * currentSpeed;
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
