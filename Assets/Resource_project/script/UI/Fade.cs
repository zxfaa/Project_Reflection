using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public GameObject targetUI;  // 需要禁用的 UI 物件
    public GameObject nextUI;

    // 禁用 UI 的方法
    public void DisableUI()
    {
        if (targetUI != null)
            targetUI.SetActive(false);  // 將目標 UI 設置為禁用狀態
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
