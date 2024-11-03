using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject NormalMenu;   // Normal 圖鑑選單
    public GameObject ExtraMenu;    // Extra 圖鑑選單

    // 切換 normal 和 extra 狀態
    public void SwitchMenu()
    {
        if (NormalMenu.activeSelf)
        {
            // 如果 normal 現在是顯示的，隱藏 normal，顯示 extra
            NormalMenu.SetActive(false);
            ExtraMenu.SetActive(true);
            Debug.Log("切換到 Extra 圖鑑");
        }
        else if (ExtraMenu.activeSelf)
        {
            // 如果 extra 現在是顯示的，隱藏 extra，顯示 normal
            ExtraMenu.SetActive(false);
            NormalMenu.SetActive(true);
            Debug.Log("切換到 Normal 圖鑑");
        }
    }
}
