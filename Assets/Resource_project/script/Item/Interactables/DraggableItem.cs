using UnityEngine;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour
{
    public PlayerMovement playerMovement; // 引用PlayerMovement腳本
    public ItemData.Item item; // 保存道具數據

    private bool isDragging = false;
    private Vector3 initialPosition;
    private Image draggingItemImage;
    private RectTransform wheelBackground;

    void Start()
    {
        draggingItemImage = GetComponent<Image>();
        wheelBackground = GetComponentInParent<RectTransform>();
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (isDragging)
        {
            DragItemWithMouse();
        }
    }

    public void StartDragging()
    {
        isDragging = true;

        // 禁用PlayerMovement組件
        if (playerMovement != null)
        {
            playerMovement.LockPlayer();
        }

        transform.SetAsLastSibling(); // 將拖曳的道具移到最前面
    }

    void DragItemWithMouse()
    {
        Vector3 mousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(wheelBackground, Input.mousePosition, Camera.main, out mousePos);
        transform.position = mousePos;

        if (Input.GetMouseButtonDown(1)) // 右鍵放下道具
        {
            StopDragging();
        }
    }

    public void StopDragging()
    {
        // 嘗試使用道具
        if (ItemUsageManager.Instance != null)
        {
            ItemUsageManager.Instance.UseItem(item, Input.mousePosition);
        }
        else
        {
            Debug.LogError("ItemUsageManager instance is not found.");
        }

        isDragging = false;
        transform.localPosition = initialPosition; // 重置位置到初始位置

        // 啟用PlayerMovement組件
        if (playerMovement != null)
        {
            playerMovement.ReleasePlayer();
        }

        
    }
}
