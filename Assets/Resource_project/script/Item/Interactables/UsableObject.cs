using UnityEngine;

public class UsableObject : MonoBehaviour
{
    public ItemData.Item requiredItem;

    public bool CanUseItem(ItemData.Item item)
    {
        return item.itemName == requiredItem.itemName;
    }

    public void UseItem(ItemData.Item item)
    {
        if (item.itemEffect == null)
        {
            Debug.LogError("Item or itemEffect is null in UsableObject.UseItem.");
            return;
        }

        if (item.itemName == requiredItem.itemName)
        {
            Debug.Log("使用道具: " + item.itemName + ", " + this.gameObject.name);

            IItemEffect effect = item.itemEffect as IItemEffect;
            if (effect != null)
            {
                effect.UseEffect(this.gameObject);
            }
            else
            {
                Debug.LogError("itemEffect is not of type IItemEffect.");
            }
        }
        else
        {
            Debug.LogError("Item name does not match the required item name.");
        }
    }
}
