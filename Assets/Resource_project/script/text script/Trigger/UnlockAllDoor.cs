using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnlockAllDoor : MonoBehaviour
{

    private bool isPlayerInRange = false;  // 用來檢查玩家是否在範圍內

    void Update()
    {
        // 檢查玩家是否在範圍內，且點擊了左鍵（滑鼠按鍵）
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Mouse0) && GetComponent<OpenDoor>().isDoorUnlocked)
        {
            UnlockAllDoors();
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;  // 玩家進入觸發區域
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;  // 玩家離開觸發區域
        }
    }
    public void UnlockAllDoors()
    {
        // 查找場景中的所有 OpenDoor 組件
        OpenDoor[] allDoors = FindObjectsOfType<OpenDoor>();

        foreach (OpenDoor door in allDoors)
        {
            // 將所有門設置為鑰匙已使用，門已解鎖
            door.isKeyUsed = true;
            door.isDoorUnlocked = true;

            // 獲取該門的 Item 組件並修改它的狀態
            Item itemVariable = door.GetComponent<Item>();
            if (itemVariable != null)
            {
                itemVariable.dialogueType = Item.DialogueType.NONE;
                itemVariable.itemType = Item.ItemType.NONE;
            }

            Debug.Log("門 " + door.name + " 已解鎖，可以進行傳送。");
        }
        TriggerPlot.TriggerCorriderToClassroom = false;
    }
}
