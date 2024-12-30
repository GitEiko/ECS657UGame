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

    public bool canPickItem()
    {
        return inventory.Count < maxSlots;
    }

    public void PickUpItem(GameObject item)
    {
        if (inventory.Count < maxSlots)
        {
            inventory.Add(item);
            UpdateInventoryUI();
        }
    }

    public void DropItem(GameObject item)
    {
        if (inventory.Count > 0)
        {
            inventory.Remove(item);
            UpdateInventoryUI();
        }
    }

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
