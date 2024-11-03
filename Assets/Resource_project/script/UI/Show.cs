using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Show : MonoBehaviour
{
    public GameObject menu = null;      //選單
    public GameObject mainMenu = null;  //按鈕
    public Player player;               // 引用玩家移動腳本
    void Start()
    {
        // 在場景中找到 Player 腳本
        player = FindObjectOfType<Player>();
    }
    public void ActiveMainMenu()
    {
        if (menu != null)         
            menu.SetActive(true);       //顯示選單        
        if (mainMenu != null)           //若此時按鈕有顯示        
            mainMenu.SetActive(false);  //使按鈕不顯示               
        if (player != null)
        {
            player.IsOpeningUI();       // 標記 UI 正在開啟，禁止移動
            Debug.Log("玩家移動已禁用");
        }
        // 限制 `EventSystem` 只處理圖鑑的 UI 事件
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Back()
    {
        if (menu != null)       
            menu.SetActive(false);       
        if (mainMenu != null)        
            mainMenu.SetActive(true);
        if (player != null)
        {
            player.IsOpeningUI();       // 標記 UI 關閉，允許移動
            Debug.Log("玩家移動已恢復");
        }
        // 限制 `EventSystem` 只處理圖鑑的 UI 事件
        EventSystem.current.SetSelectedGameObject(null);
    }
}
