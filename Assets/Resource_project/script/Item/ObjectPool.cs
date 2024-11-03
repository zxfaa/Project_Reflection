using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public List<ItemData.Item> itemTypes;  // 包含多種類型的道具
    private Dictionary<string, List<GameObject>> pooledObjects;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        pooledObjects = new Dictionary<string, List<GameObject>>();
    }

    public GameObject GetPooledObject(string itemName)
    {
        if (pooledObjects.ContainsKey(itemName))
        {
            foreach (GameObject obj in pooledObjects[itemName])
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }
        }

        // 如果池中沒有可用的物件，創建一個新的
        ItemData.Item item = itemTypes.Find(i => i.itemName == itemName);
        if (item.prefab != null)
        {
            GameObject newObj = Instantiate(item.prefab);
            newObj.SetActive(false);
            if (!pooledObjects.ContainsKey(itemName))
            {
                pooledObjects[itemName] = new List<GameObject>();
            }
            pooledObjects[itemName].Add(newObj);
            return newObj;
        }

        Debug.LogError("Item prefab not found for item: " + itemName);
        return null;
    }
}
