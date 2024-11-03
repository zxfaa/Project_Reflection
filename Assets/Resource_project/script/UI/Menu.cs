using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject inventoryPanel;
    public GameObject achievementsPanel;
    public GameObject mapPanel;

    public Button settingsButton;
    public Button inventoryButton;
    public Button achievementsButton;
    public Button mapButton;

    private void Start()
    {
        settingsButton.onClick.AddListener(() => ShowPanel(settingsPanel));
        inventoryButton.onClick.AddListener(() => ShowPanel(inventoryPanel));
        achievementsButton.onClick.AddListener(() => ShowPanel(achievementsPanel));
        mapButton.onClick.AddListener(() => ShowPanel(mapPanel));

        // 初始化時顯示設定面板
        ShowPanel(settingsPanel);
    }

    public void ShowPanel(GameObject panelToShow)
    {
        DisableAllPanels();
        panelToShow.SetActive(true);
    }

    public void DisableAllPanels()
    {
        settingsPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        achievementsPanel.SetActive(false);
        mapPanel.SetActive(false);
    }
}
