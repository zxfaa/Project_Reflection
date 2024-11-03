using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;    

public class FullScreenControl : MonoBehaviour
{
    private bool isFullScreen = false;
    public int windowedWidth = 1280;    // 預設視窗模式的寬度
    public int windowedHeight = 720;    // 預設視窗模式的高度

    void Start()
    {
        Screen.SetResolution(windowedWidth, windowedHeight, false); // 預設為視窗化
    }

    public void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen;  //切換狀態
        if (isFullScreen)
        {
            // 切換到全螢幕
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
            Debug.Log("��������ù��Ҧ�");
        }
        else
        {
            // 切換回視窗模式
            Screen.SetResolution(windowedWidth, windowedHeight, FullScreenMode.Windowed);
            Debug.Log("�����^�����Ҧ�");
        }
    }
}
