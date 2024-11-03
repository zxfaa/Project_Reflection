using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockTrigger : MonoBehaviour
{
    private bool isPlayerInRange = false;  // 用來檢查玩家是否在範圍內
    void Update()
    {
        // 檢查玩家是否在範圍內，且點擊了左鍵（滑鼠按鍵）
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Mouse0) && GetComponent<OpenDoor>().isDoorUnlocked)
        {
            UnLockTrigger();
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
    private void UnLockTrigger()
    {
        TriggerPlot.TriggerClassroomToCorrider = true;
    }
}
