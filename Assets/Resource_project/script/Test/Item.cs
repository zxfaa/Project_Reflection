using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Flower;

[RequireComponent(typeof(BoxCollider2D))]
public class Item : MonoBehaviour
{
    public enum InteractionType { NONE, PickUp, Examine, Others}
    public enum ItemType { NONE, Require, CheckBag }
    public enum DialogueType {NONE, Normal}
    [Header("屬性")]
    public InteractionType interactionType;
    public ItemType itemType;
    public DialogueType dialogueType;
    public GameObject examinePrefab;
    public RectTransform prefabContainer;
    [Header("獲得道具列表")]
    public ItemData itemData;
    public int[] itemIndices;
    [Header("道具需求")]
    public string requireItemName;
    public int requireItemIndex;
    [Header("設置物件啟用互動功能")]
    // 是否為關鍵物件
    public bool isKeyObject = false; 
    // 物件唯一標識符
    public string objectId;
    public bool canExamine = true;
    [Header("觸發效果")]
    public UnityEvent coustomEvent;
    [Header("其他")]
    // 對話索引，用於觸發特定的對話
    public int dialogueIndex;
    public GameObject instantiatedExaminePrefab;

    FlowerSystem fs;

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        gameObject.layer = 8;
    }

    void Start() 
    {
        InteractionManager.Instance.RegisterObject(this);
        fs = FlowerManager.Instance.GetFlowerSystem("default");
    }

    // 獲取或創建ExaminePrefab實例
    public GameObject GetOrCreateExaminePrefab()
    {
        if (instantiatedExaminePrefab == null)
        {
            instantiatedExaminePrefab = Instantiate(examinePrefab, prefabContainer);
        }
        return instantiatedExaminePrefab;
    }

    public void CheckAndDisableInteraction()
    {
        foreach (int index in itemIndices)
        {
            if (index >= 0 && index < itemData.items.Count)
            {
                ItemData.Item currentItem = itemData.items[index];

                if (currentItem.IsInteration)
                {
                    interactionType = InteractionType.NONE;
                    dialogueIndex = 0;
                    Debug.Log($"Item '{currentItem.itemName}' has already been interacted with. Disabling interaction.");
                }
            }
            else
            {
                Debug.LogError("Item index out of range in CheckAndDisableInteraction");
            }
        }
    }

    public void Interact()
    {
        if (!InteractionManager.Instance.CanInteract(objectId))
        {
            Debug.Log("不能與此物件互動：" + gameObject.name);
            return;
        }


        switch (interactionType)
        {
            case InteractionType.PickUp:
                PickUpItem();
                break;
            case InteractionType.Examine:
                if (canExamine)
                {
                    if (dialogueType != DialogueType.NONE)
                    {
                        TriggerDialogue(dialogueIndex);
                        dialogueType = DialogueType.NONE;
                    }
                    FindObjectOfType<InteractionSystem>().ExamineObject();
                    if (FindObjectOfType<Chest>() != null)
                        FindObjectOfType<Chest>().OpenExamine();
                } 
                break;
            case InteractionType.Others:
                OtherInteraction();
                break;
        }

        switch (itemType)
        {
            case ItemType.Require:
                if (FindObjectOfType<InventorySystem>().isDragging)
                    RequireItem(FindObjectOfType<DragAndDrop>().item.itemName);
                else if (dialogueType != DialogueType.NONE)
                    TriggerDialogue(dialogueIndex);
                break;
            case ItemType.CheckBag:
                CheckBagItems();
                break;
        }

        switch (dialogueType)
        {
            case DialogueType.Normal:
                TriggerDialogue(dialogueIndex);
                break;
        }



        InteractionManager.Instance.ObjectInteracted(objectId);
    }

   

    private void UpdateItemInteraction(int index)
    {
        //獲取並更新ItemData
        ItemData.Item currentItem = itemData.items[index];
        currentItem.IsInteration = true;
        itemData.items[index] = currentItem;

        Debug.Log($"Item '{currentItem.itemName}' interaction updated.");
    }
    #region 獲取道具
    public void PickUpItem()
    {
        // 如果有對話框，等待其結束；否則直接撿取道具
        if (dialogueType != DialogueType.NONE || !fs.isCompleted)
        {
            StartCoroutine(WaitForDialogCloseAndPickUp());
        }
        else
        {
            // 沒有對話框，直接撿取道具
            DirectPickUp();
        }
    }

    private void DirectPickUp()
    {
        // 直接進行道具拾取
        foreach (int index in itemIndices)
        {
            if (index >= 0 && index < itemData.items.Count)
            {
                // 處理完後更新IsInteration
                UpdateItemInteraction(index);
                // 優先處理EncyclopediaUI
                FindObjectOfType<InventorySystem>().PickUp(itemData.items[index]);
            }
            else
            {
                Debug.LogError("Item index out of range in DirectPickUp");
            }
        }

        // 更新圖鑑UI
        if (EncyclopediaUI.Instance != null)
        {
            EncyclopediaUI.Instance.UpdateUI();
        }
        else
        {
            Debug.LogError("EncyclopediaUI.Instance is null");
        }

        interactionType = InteractionType.NONE;

        // 撿取道具的顯示動畫
        ImageDisplayManager.Instance.QueueImagesWithFade(itemData, itemIndices);
        dialogueIndex = 0;
    }

    private IEnumerator WaitForDialogCloseAndPickUp()
    {
        // 等待對話框開啟
        yield return new WaitUntil(() => !fs.isCompleted);

        // 等待對話框關閉
        yield return new WaitUntil(() => fs.isCompleted);

        // 撿取道具
        DirectPickUp();
    }

    #endregion

    public void RequireItem(string itemName)
    {
        if (requireItemName == itemName)
        {
            //觸發設置的效果
            Debug.Log("Use Item");
            coustomEvent.Invoke();
            FindObjectOfType<DragAndDrop>().StopDragItem();
            itemType = ItemType.NONE;
        }
        else
        {
            TriggerDialogue(dialogueIndex: 14);
            FindObjectOfType<DragAndDrop>().StopDragItem();
            Debug.Log("Worng Item");
        }
    }

    void OtherInteraction()
    {
        if (GetComponent<TeleportTrigger>() != null && GetComponent<OpenDoor>().isDoorUnlocked)
        {
            TeleportTrigger tp = GetComponent<TeleportTrigger>();
            tp.Teleport();
        }
        else if (GetComponent<TransformerBox>() != null)
        {
            TransformerBox transformerBox = GetComponent<TransformerBox>();
            transformerBox.TurnOnLight();
        }
    }

    public void TriggerDialogue(int dialogueIndex)
    {
        if (fs.isCompleted)
        {
            // 從 DialogueManager 中獲取對話資料
            DialogueData dialogueData = DialogueManager.Instance.dialogueData;
            List<string> dialogueLines = dialogueData.GetDialogueByIndex(dialogueIndex);

            if (dialogueLines != null)
            {
                // 根據 SaidByPlayer 屬性選擇不同的對話框預製體
                string dialogPrefabName = dialogueData.dialogues[dialogueIndex].SaidByPlayer
                                          ? "PlayerDialogPrefab"
                                          : "EnviromentDialogPrefab";

                // 設置對話框，並將對話文本列表傳入
                fs.SetupDialog(dialogPrefabName, true, true);
                fs.SetTextList(dialogueLines);
            }
            else
            {
                Debug.LogError("Dialogue lines not found for index: " + dialogueIndex);
            }
        }
    }

    void CheckBagItems()
    {
        foreach (var item in InventorySystem.Instance.items)
        {
            if (item.index == requireItemIndex)
            {
                canExamine = true;
            }
        }
    }
}
