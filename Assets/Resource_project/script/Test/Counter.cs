using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public Item boxItem;
    public Item braceletItem;

    InventorySystem inventorySystem;
    List<ItemData.Item> itemList;

    private void Start()
    {
        inventorySystem= InventorySystem.Instance;
        itemList = inventorySystem.items;
    }

    public void BuyBox()
    {
        Image boxImage = boxItem.GetComponent<Image>();
        foreach (var item in itemList)
        {
            if (item.index == 18)
            {
                Debug.Log("Found");
                if (item.stackSize >= 6)
                {
                    Debug.Log("buy");
                    boxItem.Interact();
                    DisableImage(boxImage);
                    inventorySystem.RemoveItem(item, 6);
                    break;
                }
            }
        }
    }

    public void BuyBracelet()
    {
        bool hasItemA = false;
        bool hasItemB = false;
        Image braceletImage = braceletItem.GetComponent<Image>();
        foreach (var item in itemList)
        {
            if (item.index == 18 && item.stackSize >= 2)
            {
                Debug.Log("Found item A");
                hasItemA = true;
            }

            if (item.index == 19)
            {
                Debug.Log("Found item B");
                hasItemB = true;
            }

            if (hasItemA && hasItemB)
            {
                Debug.Log("Buy Bracelet");
                braceletItem.Interact();
                DisableImage(braceletImage);
                inventorySystem.RemoveItem(itemList.First(i => i.index == 18), 2);

                break;
            }
        }
    }

    private void DisableImage(Image image)
    {
        image.sprite = null;
        Color color = image.color;
        color.a = 0f;
        image.color = color;
    }
}
