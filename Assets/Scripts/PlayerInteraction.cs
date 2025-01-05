using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera; 
    [SerializeField] private float pickUpRange = 3f;
    [SerializeField] private float throwForce = 500f;
    [SerializeField] private Transform holdPosition;

    public GameObject pickUpText;
    public GameObject throwText;

    private AudioSource audio;

    [SerializeField] private InventorySystem inventorySystem;
    public GameObject keypadPanel;
    public GameObject paperPanel;
    [SerializeField] private KeypadController keypadController;

    private GameObject heldObject = null;
    private Rigidbody heldObjectRb;

    public GameObject crosshair;
    public GameObject crosshairInRange;

    private PlayerInput playerInput;
    private InputAction throwAction;
    private InputAction fireAction;
    private InputAction stashAction;
    private InputAction closeKeypadAction;

    // Initializes the player input actions and assigns the necessary actions for interacting with objects.
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        throwAction = playerInput.actions.FindAction("Throw");
        fireAction = playerInput.actions.FindAction("Fire");
        stashAction = playerInput.actions.FindAction("Stash");
        closeKeypadAction = playerInput.actions.FindAction("CloseKeypad");
    }

    // Subscribes to the input action events for Throw, Fire, Stash, and CloseKeypad interactions.
    void OnEnable()
    {
        throwAction.performed += OnThrow;
        fireAction.performed += OnFire;
        stashAction.performed += OnStash;
        closeKeypadAction.performed += OnCloseKeypad;
    }

    // Closes the keypad or paper panels and restores the cursor lock and player movement.
    private void OnCloseKeypad(InputAction.CallbackContext context)
    {
        if (keypadPanel.activeSelf)
        {
            keypadPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerMovement.SetCanMoveAndLookAround(true);
        }
        else if (paperPanel.activeSelf)
        {
            paperPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerMovement.SetCanMoveAndLookAround(true);
        }
    }

    // Returns the currently held object by the player, if any.
    public GameObject getHeldObject()
    {
        return heldObject;
    }

    // Unsubscribes from the input action events to avoid memory leaks.
    void OnDisable()
    {
        throwAction.performed -= OnThrow;
        fireAction.performed -= OnFire;
        stashAction.performed -= OnStash;
    }

    // Handles the crosshair state based on the player's interaction range and updates the held object position.
    void Update()
    {
        if (heldObject != null)
        {
            HoldObject();
        }
        
        if (heldObject == null) {
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("PickUp") || hit.collider.CompareTag("Door") || hit.collider.CompareTag("Keypad") || hit.collider.CompareTag("Paper") || hit.collider.CompareTag("FinalDoor"))
                {
                    crosshair.SetActive(false);
                    crosshairInRange.SetActive(true);
                }
                else
                {
                    crosshairInRange.SetActive(false);
                    crosshair.SetActive(true);
                }
            }
        }
    }

    // Tries to pick up an object within range if the player can pick it up.
    public void TryPickUpObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickUpRange))
        {
            if (hit.collider.CompareTag("PickUp") && inventorySystem.canPickItem())
            {
                PickUpObject(hit.collider.gameObject);
            }
        }
    }

    // Picks up an object, attaches it to the player's hold position, and disables its gravity and rotation.
    public void PickUpObject(GameObject pickUpObject)
    {
        heldObject = pickUpObject;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();

        heldObjectRb.useGravity = false;
        heldObjectRb.constraints = RigidbodyConstraints.FreezeRotation;
        heldObjectRb.isKinematic = true;

        heldObject.transform.position = holdPosition.position;
        heldObject.transform.parent = holdPosition;

        inventorySystem.PickUpItem(pickUpObject);
        pickUpText.SetActive(false);
        throwText.SetActive(true);
        crosshair.SetActive(true);
        crosshairInRange.SetActive(false);
    }

    // Keeps the held object at the correct position relative to the player's camera.
    public void HoldObject()
    {
        heldObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 1.5f;
    }

    // Handles the input for throwing the currently held object.
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (heldObject != null)
        {
            ThrowObject();
        }
    }

    // Throws the held object with a specified force and updates the inventory and UI accordingly.
    public void ThrowObject()
    {
        heldObject.transform.parent = null;

        heldObjectRb.isKinematic = false;
        heldObjectRb.useGravity = true;
        heldObjectRb.constraints = RigidbodyConstraints.None;

        heldObjectRb.AddForce(playerCamera.transform.forward * throwForce);

        inventorySystem.DropItem(heldObject);
        heldObject = null;
        throwText.SetActive(false);
        pickUpText.SetActive(true);

    }

    // Picks up an object and holds it at the player's position without using inventory.
    public void PullObject(GameObject pickUpObject)
    {
        heldObject = pickUpObject;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();

        heldObjectRb.useGravity = false;
        heldObjectRb.constraints = RigidbodyConstraints.FreezeRotation;
        heldObjectRb.isKinematic = true;

        heldObject.transform.position = holdPosition.position;
        heldObject.transform.parent = holdPosition;

        pickUpText.SetActive(false);
        throwText.SetActive(true);
    }

    // Handles the player's interaction with objects (pickups, doors, keypads, etc.) when firing (e.g., pressing a button).
    public void OnFire(InputAction.CallbackContext context)
    {
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickUpRange))
        {
            if (hit.collider.CompareTag("Door") && PlayerMovement.GetCanMoveAndLookAround())
            {
                ToggleDoor(hit.transform.gameObject);
            }
            else if (hit.collider.CompareTag("PickUp") && PlayerMovement.GetCanMoveAndLookAround())
            {
                if (heldObject == null)
                {
                    TryPickUpObject();
                }
            }
            else if (hit.collider.CompareTag("Keypad"))
            {
                keypadPanel.SetActive(true);
                keypadController.SetCurrentKeypad(hit.collider.gameObject.GetComponent<Keypad>());
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                PlayerMovement.SetCanMoveAndLookAround(false);
            }
            else if (hit.collider.CompareTag("Paper"))
            {
                PaperClickHandler paperHandler = hit.collider.GetComponent<PaperClickHandler>();
                if (paperHandler != null)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    PlayerMovement.SetCanMoveAndLookAround(false);
                    paperHandler.ShowMessage();
                }
            }
            else if (hit.collider.CompareTag("FinalDoor") && PlayerMovement.GetCanMoveAndLookAround())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SceneManager.LoadScene("WinningCutscene");
            }
        }
    }

    // Toggles the door's open/close state and handles its animation and interaction with the NavMesh.
    public void ToggleDoor(GameObject door)
    {
        Animator _anim = door.GetComponent<Animator>();
        NavMeshObstacle navMeshObstacle = door.GetComponent<NavMeshObstacle>();
        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        audio = door.GetComponent<AudioSource>();
        audio.PlayOneShot(audio.clip);
        if (stateInfo.IsName("DoorOpen"))
        {
            _anim.SetTrigger("CloseDoor");
            navMeshObstacle.carving = true;
        }
        else
        {
            _anim.SetTrigger("OpenDoor");
            navMeshObstacle.carving = false;
        }
    }

    // Switches the current held item with another item and updates the UI accordingly.
    public void switchItem(GameObject item)
    {
        if (heldObject != null)
        {
            heldObject.SetActive(false);
            heldObject.transform.parent = null;
            heldObjectRb = null;
            heldObject = null;
        }

        if (item != null)
        {
            heldObject = item;
            heldObject.SetActive(true);
            heldObjectRb = heldObject.GetComponent<Rigidbody>();

            heldObjectRb.useGravity = false;
            heldObjectRb.constraints = RigidbodyConstraints.FreezeRotation;
            heldObjectRb.isKinematic = true;

            heldObject.transform.position = holdPosition.position;
            heldObject.transform.parent = holdPosition;

            throwText.SetActive(true);
            pickUpText.SetActive(false);
            crosshair.SetActive(true);
            crosshairInRange.SetActive(false);
        }
        else
        {
            throwText.SetActive(false);
            pickUpText.SetActive(true);
        }
    }

    // Stashes the current held object, making it invisible and resetting the hold state.
    void OnStash(InputAction.CallbackContext context)
    {
        if (heldObject != null)
        {
            heldObject.SetActive(false);
            heldObject.transform.parent = null;
            heldObjectRb = null;
            heldObject = null;
        }
    }
}
