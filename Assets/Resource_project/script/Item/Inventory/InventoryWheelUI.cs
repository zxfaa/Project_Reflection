using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryWheelUI : MonoBehaviour
{
    public Transform wheelBackground;
    public GameObject slotPrefab;
    public int numberOfSlots = 5;
    public float radiusX = 100f;
    public float radiusY = 50f;
    public Text selectedItemName;
    public PlayerMovement playerMovement; // まノPlayerMovement竲セ

    private List<Image> slots = new List<Image>();
    private int currentIndex = 0;
    private List<ItemData.Item> items;

    void Start()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, wheelBackground);
            Image slotImage = slot.GetComponent<Image>();
            slots.Add(slotImage);
            slot.SetActive(false);
        }
        UpdateInventoryWheel();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            ScrollUp();
        }
        else if (scroll < 0f)
        {
            ScrollDown();
        }

        if (Input.GetMouseButtonDown(0))
        {
            CheckForSlotClick();
        }
    }

    void CheckForSlotClick()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(slots[i].rectTransform, Input.mousePosition, Camera.main))
            {
                StartDraggingItem(slots[i]);
                break;
            }
        }
    }

    void StartDraggingItem(Image slot)
    {
        DraggableItem draggableItem = slot.GetComponent<DraggableItem>();
        if (draggableItem != null)
        {
            draggableItem.item = items[currentIndex];
            draggableItem.StartDragging();
        }
    }

    void ScrollUp()
    {
        if (items.Count == 0) return;
        currentIndex = (currentIndex - 1 + items.Count) % items.Count;
        UpdateWheelSlots();
    }

    void ScrollDown()
    {
        if (items.Count == 0) return;
        currentIndex = (currentIndex + 1) % items.Count;
        UpdateWheelSlots();
    }

    void UpdateWheelSlots()
    {
        if (items == null || items.Count == 0)
        {
            foreach (var slot in slots)
            {
                slot.sprite = null;
                slot.gameObject.SetActive(false);
            }
            selectedItemName.text = "";
            return;
        }

        float[] angles = { 90f, 30f, 150f };
        int itemsToShow = Mathf.Min(numberOfSlots, items.Count);

        for (int i = 0; i < numberOfSlots; i++)
        {
            if (i < itemsToShow)
            {
                int itemIndex;
                if (i == 0)
                {
                    itemIndex = currentIndex;
                }
                else if (i == 1)
                {
                    itemIndex = (currentIndex + 1) % items.Count;
                }
                else
                {
                    itemIndex = (currentIndex - 1 + items.Count) % items.Count;
                    if (currentIndex == 0)
                    {
                        itemIndex = items.Count - 1;
                    }
                }

                if (itemIndex >= 0 && itemIndex < items.Count)
                {
                    slots[i].sprite = items[itemIndex].itemIcon;
                    slots[i].gameObject.SetActive(true);
                    float angle = angles[i];
                    float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radiusX;
                    float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radiusY;
                    slots[i].transform.localPosition = new Vector3(x, y, 0);
                    ResizeToFit(slots[i], items[itemIndex].itemIcon, new Vector2(65, 65)); // ﹍把计单ゑ罽

                    // Τ90竚slot砞竚DraggableItem
                    if (i == 0) // 安砞90竚琌i==0
                    {
                        DraggableItem draggableItem = slots[i].GetComponent<DraggableItem>();
                        if (draggableItem == null)
                        {
                            draggableItem = slots[i].gameObject.AddComponent<DraggableItem>();
                            draggableItem.playerMovement = playerMovement;
                        }
                    }
                    else
                    {
                        // 簿埃ㄤ竚DraggableItem
                        DraggableItem draggableItem = slots[i].GetComponent<DraggableItem>();
                        if (draggableItem != null)
                        {
                            Destroy(draggableItem);
                        }
                    }
                }
                else
                {
                    slots[i].sprite = null;
                    slots[i].gameObject.SetActive(false);
                }
            }
            else
            {
                slots[i].sprite = null;
                slots[i].gameObject.SetActive(false);
            }
        }

        if (currentIndex >= 0 && currentIndex < items.Count)
        {
            selectedItemName.text = items[currentIndex].itemName;
        }
        else
        {
            selectedItemName.text = "";
        }
    }

    public void UpdateInventoryWheel()
    {
        items = Inventory.Instance.GetItems();
        UpdateWheelSlots();
    }

    private void ResizeToFit(Image image, Sprite sprite, Vector2 initialSize)
    {
        RectTransform imageRectTransform = image.rectTransform;
        imageRectTransform.sizeDelta = initialSize; // 砞

        float imageWidth = imageRectTransform.rect.width;
        float imageHeight = imageRectTransform.rect.height;

        float spriteWidth = sprite.bounds.size.x;
        float spriteHeight = sprite.bounds.size.y;

        float scaleFactor = Mathf.Min(imageWidth / spriteWidth, imageHeight / spriteHeight);
        imageRectTransform.sizeDelta = new Vector2(spriteWidth * scaleFactor, spriteHeight * scaleFactor);
    }
}
