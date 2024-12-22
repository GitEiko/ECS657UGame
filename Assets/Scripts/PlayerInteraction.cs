using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

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
    [SerializeField] private GameObject keypadPanel;
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

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        throwAction = playerInput.actions.FindAction("Throw");
        fireAction = playerInput.actions.FindAction("Fire");
        stashAction = playerInput.actions.FindAction("Stash");
        closeKeypadAction = playerInput.actions.FindAction("CloseKeypad");
    }

    void OnEnable()
    {
        throwAction.performed += OnThrow;
        fireAction.performed += OnFire;
        stashAction.performed += OnStash;
        closeKeypadAction.performed += OnCloseKeypad;
    }

    private void OnCloseKeypad(InputAction.CallbackContext context)
    {
        keypadPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerMovement.SetCanMoveAndLookAround(true);
    }

    public GameObject getHeldObject()
    {
        return heldObject;
    }

    void OnDisable()
    {
        throwAction.performed -= OnThrow;
        fireAction.performed -= OnFire;
        stashAction.performed -= OnStash;
    }

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
                if (hit.collider.CompareTag("PickUp") || hit.collider.CompareTag("Door") || hit.collider.CompareTag("Keypad"))
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

    public void HoldObject()
    {
        heldObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 1.5f;
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (heldObject != null)
        {
            ThrowObject();
        }
    }

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
        }
    }

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
