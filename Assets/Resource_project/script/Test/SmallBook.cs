using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SmallBook : MonoBehaviour
{
    [Header("獲得道具列表")]
    public ItemData itemData;
    public int[] itemIndices;

    private string requireItem = "紙條";
    private Button button;
    private DragAndDrop dragAndDrop;
    private InventorySystem inventorySystem;
    private UnityAction action;

    private void Start()
    {
        button = GetComponent<Button>();
        dragAndDrop = FindObjectOfType<DragAndDrop>();
        inventorySystem = FindObjectOfType<InventorySystem>();
        action = () => UseItem(requireItem);
        button.onClick.AddListener(action);
    }
    public void UseItem(string itemName)
    {
        if (inventorySystem.isDragging)
        {
            if (dragAndDrop.item.itemName == itemName)
            {
                dragAndDrop.StopDragItem();
                if (itemName == "紙條")
                {
                    requireItem = "鉛筆";
                    ChangeListener();
                    PaperUsed();
                }
                else if (itemName == "鉛筆")
                {
                    requireItem = null;
                    ChangeListener();
                    PenUsed();
                }
            } 
        }
    }

    private void ChangeListener()
    {
        button.onClick.RemoveAllListeners();
        action = () => UseItem(requireItem);
        button.onClick.AddListener(action);
    }

    private void PaperUsed()
    {
        Debug.Log("Use 紙條");
    }

    private void PenUsed()
    {
        Debug.Log("Use 鉛筆");
        PickUp();
    }

    private void PickUp()
    {
        foreach (int index in itemIndices)
        {
            if (index >= 0 && index < itemData.items.Count)
            {
                // 優先處理EncyclopediaUI
                FindObjectOfType<InventorySystem>().PickUp(itemData.items[index]);

                // 處理完後更新IsInteration
                StartCoroutine(UpdateItemInteraction(index));
            }
            else
            {
                Debug.LogError("Item index out of range in PickUpItem");
            }
        }
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

    private IEnumerator UpdateItemInteraction(int index)
    {
        //確保EncyclopediaUI處理完畢
        yield return null;

        //獲取並更新ItemData
        ItemData.Item currentItem = itemData.items[index];
        currentItem.IsInteration = true;
        itemData.items[index] = currentItem;

        Debug.Log($"Item '{currentItem.itemName}' interaction updated.");
    }
}
