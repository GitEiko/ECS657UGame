using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public List<Image> slotImages; 
    public int maxSlots = 5;

    public Camera thumbnailCamera;
    public RenderTexture itemRenderTexture; 
    public Transform itemDisplayPosition;

    private List<GameObject> inventory = new List<GameObject>();
    private int currentSlotIndex = 0;
    public InputActionReference scrollAction;
    public InputActionReference switchSlotAction;
    [SerializeField] private PlayerInteraction interactionSystem;
    private float switchCooldown = 0.2f;
    private float lastSwitchTime = 0f;

    // Initializes slot images and enables input actions for scrolling and slot switching
    void Start()
    {
        foreach (var slotImage in slotImages)
        {
            slotImage.material = new Material(Shader.Find("UI/Default"));
            slotImage.material.mainTexture = null;
        }
        scrollAction.action.Enable();
        switchSlotAction.action.Enable();
    }

    // Handles inventory navigation using scroll and switch inputs, highlighting the current slot
    void Update()
    {
        Vector2 scrollValue = scrollAction.action.ReadValue<Vector2>();
        if (scrollValue.y != 0)
        {
            currentSlotIndex = (currentSlotIndex + (scrollValue.y > 0 ? -1 : 1) + maxSlots) % maxSlots;
            HighlightSlot(currentSlotIndex);
        }

        if (Time.time - lastSwitchTime > switchCooldown)
        {
            Vector2 slotSwitchInput = switchSlotAction.action.ReadValue<Vector2>();
            if (Mathf.Abs(slotSwitchInput.x) > 0.1f)
            {
                currentSlotIndex = (currentSlotIndex + (slotSwitchInput.x > 0 ? 1 : -1) + maxSlots) % maxSlots;
                HighlightSlot(currentSlotIndex);
                lastSwitchTime = Time.time;
            }
        }
    }

    // Checks if there is space available in the inventory to pick up a new item
    public bool canPickItem()
    {
        return inventory.Count < maxSlots;
    }

    // Adds an item to the inventory and updates the UI if there is space available
    public void PickUpItem(GameObject item)
    {
        if (inventory.Count < maxSlots)
        {
            inventory.Add(item);
            UpdateInventoryUI();
        }
    }

    // Removes an item from the inventory and updates the UI if the inventory is not empty
    public void DropItem(GameObject item)
    {
        if (inventory.Count > 0)
        {
            inventory.Remove(item);
            UpdateInventoryUI();
        }
    }

    // Updates the inventory slots to display thumbnails of the items or empty them if there is no item
    private void UpdateInventoryUI()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (i < inventory.Count)
            {
                DisplayItemThumbnail(inventory[i], slotImages[i]);
            }
            else
            {
                slotImages[i].sprite = null;
                Color color = slotImages[i].color;
                color.a = Mathf.Clamp01(0.2f);
                slotImages[i].color = color;
            }
        }
    }

    // Generates and assigns a thumbnail image for an item to its corresponding inventory slot
    private void DisplayItemThumbnail(GameObject item, Image slotImage)
    {
        GameObject itemCopy = Instantiate(item);
        itemCopy.transform.position = itemDisplayPosition.position;
        itemCopy.transform.localScale = item.transform.lossyScale / 2;
        itemCopy.transform.rotation = Quaternion.identity;

        itemCopy.SetActive(true);

        RenderTexture.active = itemRenderTexture;
        GL.Clear(true, true, Color.clear);

        thumbnailCamera.targetTexture = itemRenderTexture;
        thumbnailCamera.enabled = true;
        thumbnailCamera.Render();
        thumbnailCamera.enabled = false;

        Texture2D thumbnail = new Texture2D(itemRenderTexture.width, itemRenderTexture.height, TextureFormat.RGBA32, false);
        thumbnail.ReadPixels(new Rect(0, 0, itemRenderTexture.width, itemRenderTexture.height), 0, 0);
        thumbnail.Apply();

        RenderTexture.active = null;
        thumbnailCamera.targetTexture = null;
        slotImage.sprite = Sprite.Create(thumbnail, new Rect(0, 0, thumbnail.width, thumbnail.height), new Vector2(0.5f, 0.5f));
        Color color = slotImage.color;
        color.a = Mathf.Clamp01(1);
        slotImage.color = color;

        DestroyImmediate(itemCopy);
    }

    // Highlights the currently selected inventory slot and updates the player's held item
    private void HighlightSlot(int index)
    {
        for (int i = 0; i < slotImages.Count; i++)
        {
            if (i == index)
            {
                slotImages[i].transform.localScale = Vector3.one;

                if (i < inventory.Count && inventory[i] != null)
                {
                    interactionSystem.switchItem(inventory[i]);
                }
                else
                {
                    interactionSystem.switchItem(null);
                }
            }
            else
            {
                slotImages[i].transform.localScale = new Vector3(0.77018f, 0.77018f, 0.77018f);
            }
        }
    }
}
