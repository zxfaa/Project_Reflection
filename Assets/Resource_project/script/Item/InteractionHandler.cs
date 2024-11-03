using UnityEngine;
using System.Collections;
using Flower;
using System.Collections.Generic;
using Unity.VisualScripting;

public class InteractionHandler : MonoBehaviour
{
    FlowerSystem fs;
    public enum InteractionType
    {
        SimpleInteraction, // 單純互動
        PickUpItem, // 獲得道具
        UseItemOnObject //需要使用道具才能獲得道具
    }

    public InteractionType interactionType; // 交互類型
    public InteractionType postInteractionType = InteractionType.SimpleInteraction; // 互動後的交互類型
    public bool isKeyObject = false; // 是否為關鍵物件
    public string objectId; // 物件唯一標識符
    public int dialogueIndex; // 對話索引，用於觸發特定的對話
    public int postDialogueIndex;
    public bool isInteract = true;

    // 撿起道具相關
    public ItemData itemData;
    public int[] itemIndices; // 指定要撿起的道具在ItemData中的索引數組
    public bool hideObjectAfterInteraction = true; // 是否在互動後隱藏物件

    private bool playerInRange = false; // 玩家是否在範圍內
    private bool hasInteracted = false; // 是否已經進行過互動

    private void Start()
    {
        // 將自己註冊到InteractionManager中
        // InteractionManager.Instance.RegisterObject(this);
        fs = FlowerManager.Instance.GetFlowerSystem("default");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && isInteract && Input.GetMouseButtonDown(0))
        {
            if (!GameObject.Find("DialogPanel"))
            {
                Interact();
            }
        }
    }

    void Interact()
    {
        if (!InteractionManager.Instance.CanInteract(objectId))
        {
            Debug.Log("不能與此物件互動：" + gameObject.name);
            return;
        }

        if (hasInteracted)
        {
            // 已經互動過，執行簡單互動邏輯
            SimpleInteraction();
            return;
        }

        switch (interactionType)
        {
            case InteractionType.SimpleInteraction:
                SimpleInteraction();
                break;
            case InteractionType.PickUpItem:
                // 觸發對話
                TriggerDialogue(dialogueIndex);
                // 獲得道具
                PickUp();
                break;
            case InteractionType.UseItemOnObject:
                UseItemOnObject();
                break;
            default:
                Debug.LogWarning("未知的互動類型");
                break;
        }

        hasInteracted = true; // 標記為已經互動過
        interactionType = postInteractionType; // 將互動類型轉為SimpleInteraction
        dialogueIndex = postDialogueIndex;
        // 通知InteractionManager此物件已被操作
        InteractionManager.Instance.ObjectInteracted(objectId);
    }

    void SimpleInteraction()
    {
        // 實現單純互動邏輯
        Debug.Log("與" + gameObject.name + "單純互動");
        TriggerDialogue(dialogueIndex);
    }

    public void PickUp()
    {
        Debug.Log("InteractionHandler: PickUp method called");
        foreach (int index in itemIndices)
        {
            if (index >= 0 && index < itemData.items.Count)
            {
                ItemData.Item item = itemData.items[index];
                Debug.Log($"Attempting to pick up item: {item.itemName}");
                Inventory.Instance.AddItem(item);
                // 不需要單獨更新 UI，因為 AddItem 會觸發事件
            }
            else
            {
                Debug.LogError($"Item index out of range in PickUpItem: {index}");
            }
        }
        // 更新道具輪盤UI
        InventoryWheelUI inventoryWheelUI = FindObjectOfType<InventoryWheelUI>();
        if (inventoryWheelUI != null)
        {
            inventoryWheelUI.UpdateInventoryWheel();
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
        // 開始協程來等待對話框消失並執行撿拾操作
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
        if (hideObjectAfterInteraction)
        {
            gameObject.SetActive(false);
        }
    }

    void UseItemOnObject()
    {
        /*foreach (int index in itemIndices)
        {
            if (index >= 0 && index < itemData.items.Count)
            {
                ItemData.Item item = itemData.items[index];
                UsableObject usableObject = GetComponent<UsableObject>();
                if (usableObject != null)
                {
                    usableObject.UseItem(item);
                }
                else
                {
                    Debug.LogError("UsableObject is null for item: " + item.itemName);
                }
            }
            else
            {
                Debug.LogError("Item index out of range in UseItemOnObject");
            }
        }*/
    }

    public void TriggerDialogue(int dialogueIndex)
    {
        DialogueData dialogueData = DialogueManager.Instance.dialogueData;
        List<string> dialogueLines = dialogueData.GetDialogueByIndex(dialogueIndex);
        if (dialogueLines != null)
        {
            fs.SetupDialog();
            fs.SetTextList(dialogueLines);
        }
    }
}
