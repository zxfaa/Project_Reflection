using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewItemList", menuName = "Inventory/Item List")]
public class ItemData : ScriptableObject
{
    public static ItemData Instance; // 靜態實例

    [System.Serializable]
    public struct Item
    {
        public string itemName;
        public Sprite itemIcon;
        public bool IsInteration; //是否被互動過
        [TextArea]
        public string itemText; // 道具描述
        public GameObject prefab; // 道具對應的預製物件
        public ItemEffectBase itemEffect; // 道具的使用效果
        public int index;
        public int stackSize;
    }

    public List<Item> items = new List<Item>();

    private void OnEnable()
    {
        // 當腳本實例化時，將自己設置為單例實例
        Instance = this;

        // 將IsInteration改為false
        for (int i = 0; i < items.Count; i++)
        {
            Item currentItem = items[i];
            currentItem.IsInteration = false; // 重製為默認值
            items[i] = currentItem;
        }

    }

    // 添加一個方法來獲取所有道具
    public List<Item> GetAllItems()
    {
        return items;
    }

    private void OnValidate()
    {
        UpdateItemIndices();
    }

    private void UpdateItemIndices()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Item currentItem = items[i];
            currentItem.index = i;
            items[i] = currentItem;
        }
    }

    public Item GetItemByIndex(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            return items[index];
        }
        else
        {
            Debug.LogError($"Index {index} is out of range in GetItemByIndex");
            return default; // 返回一個默認的結構
        }
    }

}