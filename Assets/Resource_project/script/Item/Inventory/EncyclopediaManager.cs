using UnityEngine;

public class EncyclopediaManager : MonoBehaviour
{
    private EncyclopediaUI encyclopediaUI;  // 引用 EncyclopediaUI 腳本
    public GameObject illustratedBookPage;  // 手動設置引用

    void Start()
    {
        if (illustratedBookPage != null)
        {
            encyclopediaUI = illustratedBookPage.GetComponent<EncyclopediaUI>();

            // 將 EncyclopediaUI 與頁面顯示分離
            if (encyclopediaUI != null)
            {
                //Debug.Log("EncyclopediaUI 已找到並初始化。");
                InitializeEncyclopediaUI();
            }
            else
            {
                Debug.LogError("無法找到 EncyclopediaUI。");
            }
        }
        else
        {
            Debug.LogError("無法找到 `Illustrated Book Page`。");
        }
    }

    public void InitializeEncyclopediaUI()
    {
        if (encyclopediaUI != null)
        {
            encyclopediaUI.gameObject.SetActive(true);  // 手動激活
            encyclopediaUI.Initialize();                // 初始化
        }
    }
}
