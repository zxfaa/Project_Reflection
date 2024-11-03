using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("道具")]
    public List<ItemData.Item> items; //道具獲取列表
    [Header("道具UI")]
    public Image[] itemsImages;
    public Text itemsName;
    public Text stackText;
    [Header("道具描述UI")]
    public GameObject descriptionWindow;
    public Image descriptionImage;
    public Text descriptionTitle;
    public Text descriptionText;
    [Header("使用道具的欄位")]
    public GameObject targerObject;
    [Header("其他")]
    public bool isDragging;
    public bool isOpen;
    public event Action OnInventoryChanged;     // 物品欄變化事件，當物品欄變化時觸發
    public event Action<ItemData.Item> OnItemAdded;

    private bool isAnimating;
    private bool scrollingDirection;
    private int currentIndex = 0;
    private DragAndDrop draggableItem;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameObject dragObject = GameObject.Find("ClickArea"); // 找到獨立的拖曳物件
        draggableItem = dragObject.GetComponent<DragAndDrop>();
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
            ScrollUp();
        else if (scroll < 0f)
            ScrollDown();
        if (Input.GetMouseButtonDown(1) && isDragging)
            CheckRightClick();
    }

    public void PickUp(ItemData.Item item)
    {
        bool itemAdded = false;

        // 檢查是否有相同的道具
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == item.itemName && items[i].index == item.index)
            {
                ItemData.Item currentItem = items[i];
                currentItem.stackSize += item.stackSize;
                items[i] = currentItem;
                itemAdded = true;
                break;
            }
        }

        if (!itemAdded)
            items.Add(item);

        if (OnInventoryChanged != null)
        {
            Debug.Log("InventorySystem: Invoking OnInventoryChanged event");
            OnInventoryChanged.Invoke();
        }
        else
        {
            Debug.LogWarning("InventorySystem: OnInventoryChanged event has no subscribers");
        }

        if (OnItemAdded != null)
        {
            Debug.Log($"InventorySystem: Invoking OnItemAdded event for {item.itemName}");
            OnItemAdded.Invoke(item);
        }
        else
        {
            Debug.LogWarning("InventorySystem: OnItemAdded event has no subscribers");
        }
        Debug.Log("Item added: " + item.itemName);
        UpdateUI();
    }

    public void RemoveItem(ItemData.Item item, int quantity)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == item.itemName && items[i].index == item.index)
            {
                ItemData.Item currentItem = items[i];

                if (currentItem.stackSize > quantity)
                {
                    currentItem.stackSize -= quantity;
                    items[i] = currentItem;
                }
                else
                {
                    items.RemoveAt(i);
                }

                break;
            }
        }

        UpdateUI();
    }

    #region UI更新
    public void UpdateUI() //僅用於獲得道具與刪除道具
    {
        // 隱藏所有道具顯示
        HideAll();

        // 如果沒有道具，直接返回
        if (items == null || items.Count == 0)
        {
            foreach (var slot in itemsImages)
            {
                slot.sprite = null;
                slot.gameObject.SetActive(false);
            }
            itemsName.text = "";
            stackText.text = "";
            currentIndex = 0;
            draggableItem.draggingItemImage.enabled = false;
            return;
        }

        //draggableItem.draggingItemImage.enabled = false;

        if (currentIndex >= items.Count)
        {
            currentIndex = items.Count - 1; // 確保索引值不超過道具列表長度
            scrollingDirection = true;
            AnimateScroll();
            for (int i = 0; i < items.Count; i++)
            {
                // 設置道具圖標和顯示狀態
                itemsImages[i].sprite = items[i].itemIcon;
                itemsImages[i].transform.parent.gameObject.SetActive(true);
                itemsImages[i].gameObject.SetActive(true);
            }
            return;
        }

        // 遍歷顯示所有道具
        for (int i = 0; i < items.Count; i++)
        {
            // 設置道具圖標和顯示狀態
            itemsImages[i].sprite = items[i].itemIcon;
            itemsImages[i].transform.parent.gameObject.SetActive(true);
            itemsImages[i].gameObject.SetActive(true);
        }

        // 更新當前選中道具的名稱
        itemsName.gameObject.SetActive(true);
        itemsName.text = items[currentIndex].itemName;
        stackText.text = items[currentIndex].stackSize > 1 ? items[currentIndex].stackSize.ToString() : "";

        draggableItem.item = items[currentIndex];
        draggableItem.SetItem(items[currentIndex], itemsImages[currentIndex].sprite);  // 傳遞道具數據和圖標
        draggableItem.draggingItemImage.enabled = true;
        //itemsImages[currentIndex].gameObject.SetActive(false);
    }

    void HideAll()
    {
        foreach (var i in itemsImages)
        {
            i.gameObject.SetActive(false);
        }
        itemsName.gameObject.SetActive(false);
    }

    public void ScrollUp()
    {
        if (items.Count == 0 || isAnimating) return;
        //currentIndex = (currentIndex + 1) % items.Count;
        //UpdateUI();
        if (currentIndex < items.Count - 1)
        {
            currentIndex++;  // 減少索引值
            scrollingDirection = false;
            AnimateScroll();
        }
    }

    public void ScrollDown()
    {
        if (items.Count == 0 || isAnimating) return;
        //currentIndex = (currentIndex - 1 + items.Count) % items.Count;
        //UpdateUI();
        if (currentIndex > 0)
        {
            currentIndex--;  // 增加索引值
            scrollingDirection = true;
            AnimateScroll();
        }
    }


    private void AnimateScroll()
    {
        isAnimating = true;
        draggableItem.draggingItemImage.enabled = false;
        if (scrollingDirection)
            AnimateUIWithDOTweenDown();
        else
            AnimateUIWithDOTweenUp();

    }

    private void AnimateUIWithDOTween(bool scrollDirection)
    {
        if (scrollingDirection)  // 向下滾動
        {
            for (int i = 0; i < itemsImages.Length; i++)
            {
                RectTransform itemRectTransform = itemsImages[i].transform.parent.GetComponent<RectTransform>();
                int previousIndex = (i + 1) % itemsImages.Length;
                RectTransform previousSlotRectTransform = itemsImages[previousIndex].transform.parent.GetComponent<RectTransform>();

                // 動畫移動 UI 元素
                itemRectTransform.DOAnchorPos(previousSlotRectTransform.anchoredPosition, 0.5f).OnComplete(() =>
                {
                    isAnimating = false;  // 動畫完成標誌
                    UpdateUI();
                });
            }
        }
        else  // 向上滾動
        {
            for (int i = 0; i < itemsImages.Length; i++)
            {
                RectTransform itemRectTransform = itemsImages[i].transform.parent.GetComponent<RectTransform>();
                int nextIndex = (i - 1 + itemsImages.Length) % itemsImages.Length;
                RectTransform nextSlotRectTransform = itemsImages[nextIndex].transform.parent.GetComponent<RectTransform>();

                // 動畫移動 UI 元素
                itemRectTransform.DOAnchorPos(nextSlotRectTransform.anchoredPosition, 0.5f).OnComplete(() =>
                {
                    isAnimating = false;  // 動畫完成標誌
                    UpdateUI();
                });
            }
        }
        
        UpdateUIWithAnimate();
    }

    void UpdateUIWithAnimate()
    {
        // 更新當前選中道具的名稱與數量
        itemsName.gameObject.SetActive(true);
        itemsName.text = items[currentIndex].itemName;
        stackText.text = items[currentIndex].stackSize > 1 ? items[currentIndex].stackSize.ToString() : "";

        /*draggableItem.item = items[currentIndex];
        draggableItem.SetItem(items[currentIndex], itemsImages[currentIndex].sprite);  // 傳遞道具數據和圖標
        draggableItem.draggingItemImage.enabled = true;
        itemsImages[currentIndex].gameObject.SetActive(false);*/
    }

    private void AnimateUIWithDOTweenUp()
    {
        AnimateUIWithDOTween(false);
    }

    private void AnimateUIWithDOTweenDown()
    {
        AnimateUIWithDOTween(true);
    }
    #endregion

    #region 道具欄操控
    public void CheckLeftClick()
    {
        if (!isOpen)
            StartDraggingItem(itemsImages[currentIndex]);
    }

    public void CheckRightClick()
    {
        Debug.Log("RIGHT CLICK");
        FindObjectOfType<DragAndDrop>().StopDragItem();
        itemsImages[currentIndex].gameObject.SetActive(true);
    }

    public void StartDraggingItem(Image img)
    {
        Debug.Log("START DRAGGING");

        if (!isDragging)
        {
            if (draggableItem != null)
            {
                // 設置拖曳物件的數據
                itemsImages[currentIndex].gameObject.SetActive(false);
                draggableItem.StartDragItem();
                Debug.Log("拿起 " + items[currentIndex].itemName);
                isDragging = true;
            }
            else
            {
                Debug.Log("NULL");
            }
        }
    }

    public void ToggleExamine()
    {
        if (!isDragging)
        {
            isOpen = !isOpen;
            descriptionImage.sprite = itemsImages[currentIndex].sprite;
            descriptionTitle.text = items[currentIndex].itemName;
            descriptionText.text = items[currentIndex].itemText;
            descriptionWindow.SetActive(isOpen);
        }
    }
    #endregion

    public bool HasItem(ItemData.Item item)    // 新增方法來檢查道具是否已經獲得
    {
        bool hasItem = items.Contains(item);
        Debug.Log($"Checking if inventory has item {item.itemName}: {hasItem}");
        return hasItem;
    }

    public bool HasItem(string itemName, int index) //
    {
        foreach (var item in items)
        {
            if (item.itemName == itemName && item.index == index)
            {
                return true;
            }
        }
        return false;
    }
}
