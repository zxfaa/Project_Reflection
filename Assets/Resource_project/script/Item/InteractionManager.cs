using UnityEngine;
using System.Collections.Generic;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    private Dictionary<string, Item> objects = new Dictionary<string, Item>();
    private HashSet<string> interactedKeyObjects = new HashSet<string>();
    private Dictionary<string, List<string>> keyObjectToNormalObjects = new Dictionary<string, List<string>>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 保持跨場景存在
        }
        else
        {
            Destroy(gameObject);  // 如果已經存在實例，則銷毀物件
        }
    }

    public void RegisterObject(Item obj)
    {
        if (!objects.ContainsKey(obj.objectId))
        {
            objects.Add(obj.objectId, obj);
        }
    }

    public void RegisterKeyObject(string keyObjectId, List<string> normalObjectIds)
    {
        if (!keyObjectToNormalObjects.ContainsKey(keyObjectId))
        {
            keyObjectToNormalObjects.Add(keyObjectId, normalObjectIds);
        }
    }

    public bool CanInteract(string objectId)
    {
        if (objects[objectId].isKeyObject)
        {
            return true;
        }

        foreach (var keyObjectId in keyObjectToNormalObjects.Keys)
        {
            if (interactedKeyObjects.Contains(keyObjectId) && keyObjectToNormalObjects[keyObjectId].Contains(objectId))
            {
                return true;
            }
        }

        return false;
    }

    public void ObjectInteracted(string objectId)
    {
        if (objects[objectId].isKeyObject && !interactedKeyObjects.Contains(objectId))
        {
            interactedKeyObjects.Add(objectId);
        }
    }

    // 在Inspector中設置關鍵物件和一般物件的對應關係
    [System.Serializable]
    public struct KeyNormalObjectMapping
    {
        public string keyObjectId;
        public List<string> normalObjectIds;
    }

    public List<KeyNormalObjectMapping> keyNormalMappings;

    private void Start()
    {
        // 將Inspector中設置的關鍵物件和一般物件的對應關係註冊到Manager中
        foreach (var mapping in keyNormalMappings)
        {
            RegisterKeyObject(mapping.keyObjectId, mapping.normalObjectIds);
        }
    }
}
