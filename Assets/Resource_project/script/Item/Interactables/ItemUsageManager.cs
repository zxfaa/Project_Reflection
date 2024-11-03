using UnityEngine;

public class ItemUsageManager : MonoBehaviour
{
    public static ItemUsageManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void UseItem(ItemData.Item item, Vector3 mousePosition)
    {
        if (item.itemEffect == null)
        {
            Debug.LogError("Item or itemEffect is null in UsableObject.UseItem.");
            return;
        }
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            UsableObject usableObject = hit.collider.GetComponent<UsableObject>();
            if (usableObject != null && usableObject.CanUseItem(item))
            {
                usableObject.UseItem(item);
            }
            else if(usableObject == null) 
            {
                Debug.LogError("UsableObject is null.");
            }
            else if (!usableObject.CanUseItem(item))
            {
                Debug.LogError("Can not use item.");
            }
        }
        else
        {
            Debug.LogError("Raycast did not hit any collider.");
        }
    }
}
