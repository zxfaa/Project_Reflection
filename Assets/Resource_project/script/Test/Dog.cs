using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;

public class Doge : MonoBehaviour
{
    FlowerSystem fs;

    public static bool IsLeave =false;

    private bool isPlayerInRange = false; // 記錄玩家是否在範圍內
    private bool isInteracting = false; // 紀錄是否正在互動

    public GameObject player; // Player對象

    void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");    
    }


    void Update()
    {
        // 當玩家在範圍內且按下指定按鍵
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Mouse0) && !isInteracting)
        {
            isInteracting = true;  // 設置為正在互動
            Chat();
        }
    }

    // 當玩家進入觸發範圍時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // 玩家進入範圍
        }
    }

    // 當玩家離開觸發範圍時
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // 玩家離開範圍
        }
    }

    private void Chat()
    {
        fs.SetupUIStage();
        fs.SetupButtonGroup();

        fs.SetupButton("跟他說話", () => {
            fs.Resume();
            fs.SetupDialog("PlotDialogPrefab");
            fs.ReadTextFromResource("stage3/dog/talk");
            fs.RemoveButtonGroup();
            StartCoroutine(WaitForDialogCompletion(() =>
            {
                isInteracting = false;  // 重置交互标志
            }));
        });

        fs.SetupButton("沉默", () => {
            fs.Resume();
            fs.SetupDialog("PlotDialogPrefab");
            fs.ReadTextFromResource("stage3/dog/silence");
            fs.RemoveButtonGroup();
            StartCoroutine(WaitForDialogCompletion(() =>
            {
                isInteracting = false; 
            }));
        });

        fs.SetupButton("離開", () => {
            fs.Resume();
            fs.RemoveButtonGroup();
            fs.StopAndReset();
            StartCoroutine(WaitForDialogCompletion(() =>
            {
                isInteracting = false;  
            }));
        });
        // 查找 Player 下的 InventorySystem 组件
        InventorySystem inventorySystem = player.GetComponentInChildren<InventorySystem>();
        //這裡放上有沒有蛋糕
        if (inventorySystem.items.Exists(item => item.index == 25))
        {
            fs.SetupButton("拿出蛋糕", () => {
                fs.Resume();
                fs.SetupDialog("PlotDialogPrefab");
                fs.ReadTextFromResource("stage3/dog/cake");
                IsLeave = true;
                fs.RemoveButtonGroup();
                StartCoroutine(WaitForDialogCompletion(() =>
                {
                    isInteracting = false;
                }));
                Debug.Log($"{IsLeave}");
                //把互動的trigger移除
            });
        }
        
    }

    private IEnumerator WaitForDialogCompletion(System.Action onCompleted)
    {
        // 等待對話完成
        while (!fs.isCompleted)
        {
            yield return null; // 每幀檢查一次
        }

        // 對話完成後執行回調
        onCompleted?.Invoke();
    }
}
