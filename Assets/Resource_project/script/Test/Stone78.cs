using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Flower;

public class Stone78 : MonoBehaviour
{
    [Header("檢查道具")]
    public ItemData itemData;
    public int[] itemIndices;

    private Image image;
    private Item tableType;
    private Item classroomType;
    FlowerSystem fs;

    private void Start()
    {
        image = this.GetComponent<Image>();
        GameObject table = GameObject.Find("LaboratoryTable");
        GameObject classtable = GameObject.Find("GA Seat");
        tableType = table.GetComponent<Item>();
        classroomType = classtable.GetComponent<Item>();
        fs = FlowerManager.Instance.GetFlowerSystem("default");
    }

    public void Put()
    {
        if (FindObjectOfType<InventorySystem>().isDragging)
        {
            PutItem(FindObjectOfType<DragAndDrop>().item);
            return;
        }
    }

    private void PutItem(ItemData.Item item)
    {
        Debug.LogWarning($"put item {item.itemName} {item.index}");
        if (image.sprite == null)
        {
            if (item.index == 15 || item.index == 16)
            {
                itemIndices[0] = item.index;
                image.sprite = item.itemIcon;
                Color color = image.color;
                color.a = 255f;
                image.color = color;
                fs.SetupDialog("PlayerDialogPrefab");
                fs.SetTextList(new List<string>{"第八顆...應該也要進得去才對...[w][remove_dialog]"});
                Debug.Log("觸發對話");
            }
        }
        else if (image.sprite != null && (itemIndices[0] == 15 || itemIndices[0] == 16))
        {
            Debug.Log("Trigger Animate");
            tableType.interactionType = Item.InteractionType.PickUp;
            classroomType.interactionType = Item.InteractionType.PickUp;
            classroomType.dialogueIndex = 1;
        }
        FindObjectOfType<DragAndDrop>().StopDragItem();
        int quantityToRemove = 1;
        FindObjectOfType<InventorySystem>().RemoveItem(item, quantityToRemove);
    }
}
