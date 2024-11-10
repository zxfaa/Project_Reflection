using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Fade : MonoBehaviour
{
    public GameObject targetUI;  // 需要禁用的 UI 物件
    public GameObject nextUI;
    public Player player;

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();   // 自動找到玩家腳本
            player.LockMovement(true);   // 鎖定移動
        }
    }

    // 禁用 UI 的方法
    public void DisableUI()
    {
        if (targetUI != null)
            targetUI.SetActive(false);  // 將目標 UI 設置為禁用狀態
        if (player != null)
            player.LockMovement(false);   // 解除鎖定移動
    }

    public void NextUI()
    {
        if(nextUI != null)
        {
            targetUI.SetActive(false);
            nextUI.SetActive(true);
        }           
    }
}
