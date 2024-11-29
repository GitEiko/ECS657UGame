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
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float stepDistance = 2f;
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> defaultFootstepSounds;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Dictionary<int, List<AudioClip>> layerFootstepSounds = new Dictionary<int, List<AudioClip>>();

    float verticalVelocity = 0f;
    float xRotation = 0f;
    bool isSprinting = false;

    CharacterController characterController;
    Vector3 lastFootstepPosition;

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

        lastFootstepPosition = transform.position;
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
        HandleFootsteps();
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

    void HandleFootsteps()
    {
        if (!characterController.isGrounded) return;

        Vector3 currentPosition = transform.position;
        float distanceMoved = Vector3.Distance(lastFootstepPosition, currentPosition);

        if (distanceMoved >= stepDistance)
        {
            PlayFootstepSound();
            lastFootstepPosition = currentPosition;
        }
    }

    void PlayFootstepSound()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer))
        {
            int layer = hit.collider.gameObject.layer;

            if (layerFootstepSounds.ContainsKey(layer) && layerFootstepSounds[layer].Count > 0)
            {
                PlayRandomSound(layerFootstepSounds[layer]);
            }
            else
            {
                PlayRandomSound(defaultFootstepSounds);
            }
        }
        else
        {
            PlayRandomSound(defaultFootstepSounds);
        }
    }

    void PlayRandomSound(List<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0 || audioSource == null) return;

        AudioClip clip = clips[Random.Range(0, clips.Count)];
        audioSource.PlayOneShot(clip);
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
