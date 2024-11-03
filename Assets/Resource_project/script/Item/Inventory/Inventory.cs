using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }    // 靜態變量，用於存儲 Inventory 類的單例實例

    [SerializeField]     // 用於存儲物品的私有列表
    private List<ItemData.Item> items = new List<ItemData.Item>();

    [SerializeField]
    private ItemData itemData; // 引用 ItemData ScriptableObject

    public event Action OnInventoryChanged;    // 物品欄變化事件，當物品欄變化時觸發
    public event Action<ItemData.Item> OnItemAdded;

    private void Awake() // Awake 方法在腳本實例化時調用
    {
        Debug.Log("Inventory Awake called");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("Inventory Start called");
    }

    public List<ItemData.Item> GetItems()    // 獲取物品列表，返回列表的副本以保護原始數據
    {
        return new List<ItemData.Item>(items); // 返回物品列表的副本
    }

   
    public void AddItem(ItemData.Item item) // 添加物品並觸發變化事件
    {
        Debug.Log($"Inventory: Attempting to add item: {item.itemName}");
        if (!items.Contains(item))
        {
            items.Add(item);
            Debug.Log($"Inventory: Item added: {item.itemName}");
            
            if (OnInventoryChanged != null)
            {
                Debug.Log("Inventory: Invoking OnInventoryChanged event");
                OnInventoryChanged.Invoke();
            }
            else
            {
                Debug.LogWarning("Inventory: OnInventoryChanged event has no subscribers");
            }
            
            if (OnItemAdded != null)
            {
                Debug.Log($"Inventory: Invoking OnItemAdded event for {item.itemName}");
                OnItemAdded.Invoke(item);
            }
            else
            {
                Debug.LogWarning("Inventory: OnItemAdded event has no subscribers");
            }
        }
        else
        {
            Debug.Log($"Inventory: Item already in inventory: {item.itemName}");
        }
    }
    /*{
        items.Add(item); // 將新物品添加到列表中
        OnInventoryChanged?.Invoke(); // 觸發物品欄變化事件（如果有訂閱者）
        OnItemAdded?.Invoke(item);
        Debug.Log("Item added: " + item.itemName); // 輸出日誌，顯示添加的物品名稱
    }*/

    public void RemoveItem(ItemData.Item item)    // 移除物品並觸發變化事件
    {
        items.Remove(item); // 將物品從列表中移除
        OnInventoryChanged?.Invoke(); // 觸發物品欄變化事件（如果有訂閱者）        
    }

    public bool HasItem(ItemData.Item item)    // 新增方法來檢查道具是否已經獲得
    {
        if (item.Equals(default(ItemData.Item)))
        {
            return false;
        }
        bool hasItem = items.Contains(item);
        Debug.Log($"Checking if inventory has item {item.itemName}: {hasItem}");
        return hasItem;
    }
    /*{
        return items.Contains(item);
    }*/
}
