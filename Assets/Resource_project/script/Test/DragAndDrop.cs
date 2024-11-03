using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour
{
    public ItemData.Item item; // 保存道具數據

    private Vector3 initialPosition;
    public Image draggingItemImage;
    private RectTransform itemRectTransform;

    void Start()
    {
        draggingItemImage = GetComponent<Image>();
        draggingItemImage.enabled = false;
        itemRectTransform = GetComponentInParent<RectTransform>();
        initialPosition = transform.localPosition; 
    }

    public void SetItem(ItemData.Item newItem, Sprite icon)
    {
        // 設置道具數據和圖標
        item = newItem;
        draggingItemImage.sprite = icon;
    }

    // 到時候測試如果出現特殊形況角色能動的話這裡補回
    /*void Update()
    {
        if (FindObjectOfType<InventorySystem>().isDragging)
            DraggingItem();
    }*/

    public void StartDragItem()
    {
        draggingItemImage.raycastTarget = false;
        transform.SetAsLastSibling(); // 將拖曳的道具移到最前面  
    }

    public void DraggingItem()
    {
        Vector3 mousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(itemRectTransform, Input.mousePosition, Camera.main, out mousePos);
        transform.position = mousePos;
    }

    public void StopDragItem()
    {
        draggingItemImage.raycastTarget = true;
        transform.localPosition = initialPosition; // 重置位置到初始位置
        FindObjectOfType<InventorySystem>().isDragging = false;
    }

    internal void SetItem(object value1, object value2)
    {
        throw new NotImplementedException();
    }
}
