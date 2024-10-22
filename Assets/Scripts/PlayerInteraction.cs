using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera; 
    [SerializeField] private float pickUpRange = 3f;
    [SerializeField] private float throwForce = 500f;
    [SerializeField] private Transform holdPosition;

    public GameObject pickUpText;
    public GameObject throwText;

    private GameObject heldObject = null;
    private Rigidbody heldObjectRb;

    private PlayerInput playerInput;
    private InputAction throwAction;
    private InputAction fireAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        throwAction = playerInput.actions.FindAction("Throw");
        fireAction = playerInput.actions.FindAction("Fire");
    }

    void OnEnable()
    {
        throwAction.performed += OnThrow;
        fireAction.performed += OnFire;
    }

    void OnDisable()
    {
        throwAction.performed -= OnThrow;
        fireAction.performed -= OnFire;
    }

    void Update()
    {
        if (heldObject != null)
        {
            HoldObject();
        }
    }

    public void TryPickUpObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickUpRange))
        {
            if (hit.collider.CompareTag("PickUp"))
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

        pickUpText.SetActive(false);
        throwText.SetActive(true);
    }

    public void HoldObject()
    {
        heldObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 1.5f; // 1.5f is the distance from the camera, adjust as needed
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

        heldObject = null;
        throwText.SetActive(false);
        pickUpText.SetActive(true);

    }

    public void OnFire(InputAction.CallbackContext context)
    {
        Debug.Log("Object pick up called");
        if (heldObject == null)
        {
            Debug.Log("Object picked up");
            TryPickUpObject();
        }
    }
}
