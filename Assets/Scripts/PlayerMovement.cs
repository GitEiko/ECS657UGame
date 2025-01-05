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

    float speed;
    float sprintSpeed;
    //[SerializeField] float speed = 5f;
    //[SerializeField] float sprintSpeed = 10f;
    [SerializeField] float lookSensitivity = 1f;
    [SerializeField] Transform playerCamera;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float stepDistance = 2f;
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> defaultFootstepSounds;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Dictionary<int, List<AudioClip>> layerFootstepSounds = new Dictionary<int, List<AudioClip>>();

    public EnemyNavigation enemyNavigation;
    float verticalVelocity = 0f;
    float xRotation = 0f;
    bool isSprinting = false;
    public List<Collider> colliders = new List<Collider>();
    private static bool canMoveAndLookAround;

    CharacterController characterController;
    Vector3 lastFootstepPosition;

    // Initializes movement, sprinting, and camera controls.
    void Start()
    {
        speed = GameSettings.Instance.PlayerSpeed;
        sprintSpeed = speed + 8;
        canMoveAndLookAround = true;
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

    // Applies gravity to the player, adjusting vertical velocity when grounded or airborne.
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

    // Changes the enemy state to Patrol when the player enters a trigger collider.
    void OnTriggerEnter(Collider other)
    {
        if (colliders.Contains(other))
        {
            enemyNavigation.setCurrentState(EnemyNavigation.EnemyState.Patrol);
        }
    }

    // Changes the enemy state to Chase when the player exits a trigger collider.
    void OnTriggerExit(Collider other)
    {
        if (colliders.Contains(other))
        {
            enemyNavigation.setCurrentState(EnemyNavigation.EnemyState.Chase);
        }
    }

    // Handles player movement, looking around, and footstep sounds, if the player can move and look around.
    void Update()
    {
        if (canMoveAndLookAround)
        {
            MovePlayer();
            LookAround();
            HandleFootsteps();
        }
        else
        {
            characterController.Move(Vector3.zero);
        }
        ApplyGravity();
    }

    // Moves the player based on input and applies sprinting if active.
    void MovePlayer()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        float currentSpeed = isSprinting ? sprintSpeed : speed;

        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;

        moveDirection.y = verticalVelocity;

        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    // Rotates the player based on input to control camera orientation.
    void LookAround()
    {
        Vector2 lookVector = lookAction.ReadValue<Vector2>();

        transform.Rotate(Vector3.up * lookVector.x * lookSensitivity);

        xRotation -= lookVector.y * lookSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // Checks the player's movement distance and triggers footstep sounds.
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

    // Plays an appropriate footstep sound based on the surface the player is walking on.
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

    // Plays a random footstep sound from the provided list of clips.
    void PlayRandomSound(List<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0 || audioSource == null) return;

        AudioClip clip = clips[Random.Range(0, clips.Count)];
        audioSource.PlayOneShot(clip);
    }

    // Starts the sprinting state.
    void StartSprinting()
    {
        isSprinting = true;
    }

    // Stops the sprinting state.
    void StopSprinting()
    {
        isSprinting = false;
    }

    // Sets whether the player can move and look around.
    public static void SetCanMoveAndLookAround(bool val)
    {
        canMoveAndLookAround = val;
    }

    // Returns whether the player can move and look around.
    public static bool GetCanMoveAndLookAround()
    {
        return canMoveAndLookAround;
    }
}
