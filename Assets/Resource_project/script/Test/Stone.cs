using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stone : MonoBehaviour
{
    [Header("獲得道具列表")]
    public ItemData itemData;
    public int[] itemIndices;
    public int correctItemIndex;
    public bool isItemCorrectlyPlaced = false;

    private Image image;
    private Chest chestManager;

    private void Start()
    {
        image = this.GetComponent<Image>();
    }

    public void SetChestManager(Chest manager)
    {
        chestManager = manager;
    }

    public void PickChestItem()
    {
        for (int i = 0; i < itemIndices.Length; i++)
        {
            int index = itemIndices[i];
            if (index >= 0 && index < itemData.items.Count && image.sprite != null)
            {
                InventorySystem.Instance.PickUp(itemData.items[index]);
                image.sprite = null;
                Color color = image.color;
                color.a = 0f;
                image.color = color;
                isItemCorrectlyPlaced = false;
            }
            else if (FindObjectOfType<InventorySystem>().isDragging)
            {
                PutItem(FindObjectOfType<DragAndDrop>().item);
                return;
            }
            else
            {
                return;
            }
        }
        
        FindObjectOfType<InventorySystem>().UpdateUI();
    }

    public void PickLibraItem()
    {
        foreach (int index in itemIndices)
        {
            if (index >= 0 && index < itemData.items.Count)
            {
                InventorySystem.Instance.PickUp(itemData.items[index]);
            }
            else
            {
                return;
            }
        }

        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        FindObjectOfType<InventorySystem>().UpdateUI();
        gameObject.SetActive(false);
    }

    public void LibraStone()
    {
        for (int i = 0; i < itemIndices.Length; i++)
        {
            int index = itemIndices[i];
            if (index >= 0 && index < itemData.items.Count && image.sprite != null)
            {
                InventorySystem.Instance.PickUp(itemData.items[index]);
                image.sprite = null;
                Color color = image.color;
                color.a = 0f;
                image.color = color;
                isItemCorrectlyPlaced = false;
                FindObjectOfType<LibraConrtoller>().stoneGram = 0;
                FindObjectOfType<LibraConrtoller>().LibraAnimate();
            }
            else if (FindObjectOfType<InventorySystem>().isDragging)
            {
                PutItem(FindObjectOfType<DragAndDrop>().item);
                return;
            }
            else
            {
                return;
            }
        }

        FindObjectOfType<InventorySystem>().UpdateUI();
    }

    private void UpdateUI()
    {
        // 更新圖鑑UI
        if (EncyclopediaUI.Instance != null)
        {
            Debug.Log("InteractionHandler: EncyclopediaUI.Instance found, calling UpdateUI");
            EncyclopediaUI.Instance.UpdateUI();
        }
        else
        {
            Debug.LogError("InteractionHandler: EncyclopediaUI.Instance is null");
        }
        StartCoroutine(WaitForDialogToCloseAndPickUpItem());    
    }

    private IEnumerator WaitForDialogToCloseAndPickUpItem()
    {
        // 等待一段時間，以確保對話框已經開始顯示
        yield return new WaitForSeconds(0.1f); // 短暫等待

        // 檢查對話框是否存在
        while (GameObject.Find("DialogPanel"))
        {
            yield return null; // 每幀檢查一次
        }

        // 對話框消失後，執行撿拾操作        
        ImageDisplayManager.Instance.QueueImagesWithFade(itemData, itemIndices);
    }

    private void PutItem(ItemData.Item item)
    {
        Debug.LogWarning($"put item {item.itemName} {item.index}");
        for (int i = 0; i < itemIndices.Length; i++)
        {
            itemIndices[i] = item.index;
            if (image.sprite == null)
            {
                image.sprite = item.itemIcon;
                Color color = image.color;
                color.a = 255f;
                image.color = color;
                if (itemIndices[i] == correctItemIndex)
                {
                    isItemCorrectlyPlaced = true;
                    chestManager.CheckAllSlots();
                }
            }
            Debug.LogWarning(itemIndices[i]);
        }
        FindObjectOfType<LibraConrtoller>().SetStoneGram();
        FindObjectOfType<DragAndDrop>().StopDragItem();
        int quantityToRemove = 1;
        FindObjectOfType<InventorySystem>().RemoveItem(item, quantityToRemove);
    }
}
