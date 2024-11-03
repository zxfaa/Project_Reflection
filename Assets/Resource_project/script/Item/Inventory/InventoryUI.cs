using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    // 物品UI的預製件
    public GameObject itemUIPrefab;
    // 物品UI的父節點
    public Transform itemsParent;
    // 用於存儲創建的物品UI實例
    private List<GameObject> itemUIInstances = new List<GameObject>();

    // 當腳本啟用時調用
    private void OnEnable()
    {
        // 確認 Inventory 實例不為 null
        if (Inventory.Instance != null)
        {
            // 訂閱物品欄變化事件
            Inventory.Instance.OnInventoryChanged += UpdateInventoryUI;
            // 初始更新UI
            UpdateInventoryUI();
        }
    }

    // 當腳本禁用時調用
    private void OnDisable()
    {
        // 確認 Inventory 實例不為 null
        if (Inventory.Instance != null)
        {
            // 取消訂閱物品欄變化事件
            Inventory.Instance.OnInventoryChanged -= UpdateInventoryUI;
        }
    }

    // 更新物品欄UI
    private void UpdateInventoryUI()
    {
        // 清除現有的UI項
        foreach (GameObject itemUI in itemUIInstances)
        {
            Destroy(itemUI); // 銷毀每個物品UI實例
        }
        itemUIInstances.Clear(); // 清空列表

        // 獲取物品欄中的所有物品
        List<ItemData.Item> items = Inventory.Instance.GetItems();
        // 為每個物品創建UI實例
        foreach (ItemData.Item item in items)
        {
            GameObject itemUI = Instantiate(itemUIPrefab, itemsParent); // 創建物品UI實例
            itemUI.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.itemIcon; // 設置物品圖標
            itemUI.transform.Find("ItemName").GetComponent<Text>().text = item.itemName; // 設置物品名稱
            itemUIInstances.Add(itemUI); // 將UI實例添加到列表中
        }
    }
}
