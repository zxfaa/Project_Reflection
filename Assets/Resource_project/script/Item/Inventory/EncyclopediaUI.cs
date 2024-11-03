using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;

[System.Serializable]
public class Chapter
{
    public string chapterName;           // 章節名稱
    public List<ItemData.Item> items;    // 章節包含的道具
    public Button unlockButton;          // 章節對應的按鈕
    public Button plotButton;            // 劇情按鈕，需控制其鎖定狀態
}

public class EncyclopediaUI : MonoBehaviour
{    
    public static EncyclopediaUI Instance { get; private set; }                                           // 圖鑑的單例實例    
    public bool IsInitialized { get; private set; } = false;                                              // 確保圖鑑只初始化一次的標誌    
    private Dictionary<Tuple<string, int>, Image> itemSlots = new Dictionary<Tuple<string, int>, Image>();// 用於保存道具的圖鑑槽
    [SerializeField] private ItemData itemData;                                                           // 引用 ItemData ScriptableObject
    public List<Chapter> chapters;                                                                        // 所有章節及其對應的道具和按鈕
    public Image[] itemImages;                                                                            // 引用 Image 組件
    // 新增的 Extra 檢測區塊
    public Image[] extraItemImages; // Extra區塊的 UI 方塊
    [SerializeField] private List<int> extraKeyItemIndexes; // 直接存儲 Extra 區塊要檢測的三個道具 index
    public static int ExtraCounter { get; private set; }
    public void Initialize()
    {
        if (IsInitialized) return;              // 防止重複初始化
        //Debug.Log("EncyclopediaUI 已初始化");   // 圖鑑初始化邏輯（只執行一次）
        IsInitialized = true;

        InitializeSlots();                      // 初始化圖鑑槽
        // 檢查是否正確初始化所有道具槽位                                        
        if (itemSlots == null || itemSlots.Count == 0)
            Debug.LogError("圖鑑槽初始化失敗。");
    }

    public static class EncyclopediaProgress
    {       
        public static Dictionary<string, bool> unlockedChapters = new Dictionary<string, bool>();// 靜態變數保存章節解鎖狀態       
        public static Dictionary<string, bool> collectedItems = new Dictionary<string, bool>();  // 靜態變數保存道具的拾取狀態（使用道具名稱作為鍵）
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);            // 防止多個圖鑑實例
    }

    void Start()
    {
        if (itemData == null)
        {
            Debug.LogError("ItemData is not assigned in the Inspector");
            return;
        }

        InitializeSlots();       // 初始化圖鑑槽
        SubscribeToEvents();     // 訂閱事件
        UpdateUI();              // 更新 UI 狀態

        foreach (var chapter in chapters)           // 初始化時隱藏所有按鈕
            chapter.unlockButton.gameObject.SetActive(false); 
        
        gameObject.SetActive(false);
        InitializeExtra();

        RestoreCollectedItems();            // 恢復道具顯示狀態
        RestoreUnlockedChapters();          // 恢復已解鎖的章節狀態
    }

    // 初始化圖鑑槽位
    void InitializeSlots()
    {
        IsInitialized = true;  // 標記為已初始化
        List<ItemData.Item> allItems = itemData.GetAllItems();

        //Debug.Log($"Initializing {allItems.Count} item slots.");

        for (int i = 0; i < Mathf.Min(allItems.Count, itemImages.Length); i++)
        {
            ItemData.Item item = allItems[i];
            Image itemImage = itemImages[i];

            // 使用 item 名稱與 index 作為鍵
            var key = new Tuple<string, int>(item.itemName, item.index);
            itemSlots[key] = itemImage;

            // 初始化時將所有道具圖標設置為灰色
            itemImage.sprite = item.itemIcon;
            itemImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);          // 灰色
            itemImage.gameObject.SetActive(true);                       // 確保方框是啟用狀態
        }
    }

    // 訂閱事件
    void SubscribeToEvents()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged += UpdateUI;
            InventorySystem.Instance.OnItemAdded += OnItemAdded;
        }
        else
        {
            Debug.LogError("InventorySystem.Instance is null when trying to subscribe to events");
            StartCoroutine(RetrySubscribeToEvents());
        }
    }

    // 重複偵測事件
    private IEnumerator RetrySubscribeToEvents()
    {
        while (InventorySystem.Instance == null)
            yield return new WaitForSeconds(1f);            // 每秒重試一次

        InventorySystem.Instance.OnInventoryChanged += UpdateUI;
        InventorySystem.Instance.OnItemAdded += OnItemAdded;
    }

    private void OnEnable() => SubscribeToEvents();

    private void OnDisable()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged -= UpdateUI;
            InventorySystem.Instance.OnItemAdded -= OnItemAdded;
        }
    }

    //拾取道具後圖鑑的變化
    private void OnItemAdded(ItemData.Item item)
    {
        var key = new Tuple<string, int>(item.itemName, item.index);

        // 檢查鍵值是否存在
        if (itemSlots.TryGetValue(key, out Image itemImage))
        {
            itemImage.sprite = item.itemIcon;
            itemImage.color = Color.white;      // 顯示道具圖標
            Debug.Log($"道具已更新：{item.itemName}，圖標已設置。");

            // 記錄道具已拾取狀態
            if (!EncyclopediaProgress.collectedItems.ContainsKey(item.itemName))
                EncyclopediaProgress.collectedItems[item.itemName] = true;      // 標記該道具已拾取
        }
        else
            Debug.LogError($"找不到該道具的插槽: {item.itemName}，index: {item.index}");

        CheckAllItemsCollected();       // 檢查是否收集完所有道具
        UpdateExtraUI();
        IncrementExtraCounter();
    }

    public void UpdateUI()
    {
        foreach (var pair in itemSlots)
        {
            var key = pair.Key;
            Image itemImage = pair.Value;

            bool hasItem = InventorySystem.Instance.HasItem(key.Item1, key.Item2);
            if (hasItem)
            {
                itemImage.sprite = itemData.GetItemByIndex(key.Item2).itemIcon;
                itemImage.color = Color.white;                      // 顯示為正常顏色
            }
            else
                itemImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);  // 保持灰色
        }
        CheckAllItemsCollected();
    }
    public void RestoreItemImages()
    {
        foreach (var pair in itemSlots)
        {
            var key = pair.Key;
            Image itemImage = pair.Value;

            if (EncyclopediaProgress.collectedItems.ContainsKey(key.Item1) && EncyclopediaProgress.collectedItems[key.Item1])
            {
                ItemData.Item item = itemData.GetItemByIndex(key.Item2);
                // 修改这里的 null 检查
                itemImage.sprite = item.itemIcon;
                itemImage.color = Color.white;  // 显示正常颜色
            }
            else
            {
                itemImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);  // 如果道具未被收集，保持灰色
            }
        }
    }

    /*---------------------------------------水仙花跟圖鑑-------------------------------------------------*/
    private bool isNarcissusUsed = false;

    // 針對特定章節進行鎖定
    public void LockPlotButtonsAfterNarcissus(string chapterNameToLock)
    {
        isNarcissusUsed = true;  // 標記水仙花已使用
        var chapterToLock = chapters.Find(chapter => chapter.chapterName == chapterNameToLock);// 查找指定的章節

        if (chapterToLock != null)
        {
            // 鎖定特定章節的劇情按鈕和解鎖按鈕
            chapterToLock.plotButton.interactable = false;
            SetButtonColor(chapterToLock.plotButton, Color.gray);

            chapterToLock.unlockButton.interactable = false;
            SetButtonColor(chapterToLock.unlockButton, Color.gray);

            Debug.Log($"章節 {chapterNameToLock} 的按鈕已鎖定（使用水仙花後）。");
        }
        else
            Debug.LogError($"找不到名為 {chapterNameToLock} 的章節。");
    }

    /*--------------------------------------解鎖按鈕與劇情按鈕控制----------------------------------------*/
    void CheckAllItemsCollected()
    {
        foreach (var chapter in chapters)
        {
            bool allItemsCollected = true;

            foreach (var item in chapter.items)
            {
                var key = new Tuple<string, int>(item.itemName, item.index);
                bool hasItem = InventorySystem.Instance.HasItem(item.itemName, item.index);
                
                if (!hasItem)
                {
                    allItemsCollected = false;
                    //Debug.Log($"缺少道具：{item.itemName}，位於章節：{chapter.chapterName}");
                    break;
                }
            }

            if (allItemsCollected)      //對已收集完畢之章節做靜態保存
            {
                chapter.unlockButton.gameObject.SetActive(true);    // 顯示按鈕
                chapter.unlockButton.interactable = true;           // 確保按鈕可交互
                Debug.Log($"章節 {chapter.chapterName} 的所有道具已經收集，按鈕已啟用。");

                // 記錄已解鎖章節狀態
                if (!EncyclopediaProgress.unlockedChapters.ContainsKey(chapter.chapterName))
                {
                    EncyclopediaProgress.unlockedChapters[chapter.chapterName] = true;
                }
            }
            else
            {
                chapter.unlockButton.gameObject.SetActive(false);   // 隱藏按鈕
                chapter.unlockButton.interactable = false;          // 禁用按鈕交互
            }

            if (allItemsCollected && !isNarcissusUsed)      // 如果道具已收集完全且水仙花未使用，解鎖按鈕
            {
                chapter.plotButton.interactable = true;             //外層按鈕開啟
                SetButtonColor(chapter.plotButton, Color.white);    //顏色設置

                chapter.unlockButton.gameObject.SetActive(true);    //內層按鈕開啟
                chapter.unlockButton.interactable = true;           //內層按鈕交互開啟
                SetButtonColor(chapter.unlockButton, Color.white);  //顏色設置

                Debug.Log($"章節 {chapter.chapterName} 的按鈕已啟用。");
            }
            else if (!allItemsCollected && !isNarcissusUsed)
            {
                chapter.plotButton.interactable = true;             //外層按鈕開啟
                SetButtonColor(chapter.plotButton, Color.white);    //顏色設置

                chapter.unlockButton.gameObject.SetActive(false);   //內層按鈕開啟
                chapter.unlockButton.interactable = false;          //內層按鈕交互開啟
                SetButtonColor(chapter.unlockButton, Color.white);  //顏色設置
            }
            else //東西沒拿完還偷吃步 鎖圖鑑
            {
                chapter.plotButton.interactable = false;            //外層按鈕關閉
                SetButtonColor(chapter.plotButton, Color.gray);     //顏色設置

                chapter.unlockButton.interactable = false;          //內層按鈕關閉
                SetButtonColor(chapter.unlockButton, Color.gray);   //顏色設置

                Debug.Log($"章節 {chapter.chapterName} 的按鈕已鎖定。");
            }
        }
    }

    public void SetButtonColor(Button button, Color color)          // 設置按鈕顏色
    {
        var colors = button.colors;
        colors.disabledColor = color;   // 設置禁用時的顏色
        button.colors = colors;         // 應用新的顏色
    }

    /*-------------------------------------------------道具跨場景--------------------------------------------------------*/
    void RestoreCollectedItems()
    {
        foreach (var pair in itemSlots)
        {
            var key = pair.Key;
            Image itemImage = pair.Value;

            // 如果道具已經被拾取，顯示道具圖標
            if (EncyclopediaProgress.collectedItems.ContainsKey(key.Item1) && EncyclopediaProgress.collectedItems[key.Item1])
            {
                itemImage.sprite = itemData.GetItemByIndex(key.Item2).itemIcon;
                itemImage.color = Color.white;  // 顯示正常顏色
            }
            else               
                itemImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);      // 如果道具未被拾取，保持灰色
        }
    }

    void RestoreUnlockedChapters()
    {
        foreach (var chapter in chapters)
        {
            if (EncyclopediaProgress.unlockedChapters.ContainsKey(chapter.chapterName) && EncyclopediaProgress.unlockedChapters[chapter.chapterName])
            {
                chapter.unlockButton.gameObject.SetActive(true);  // 如果該章節已解鎖，顯示按鈕
                chapter.unlockButton.interactable = true;         // 確保按鈕可交互
            }
            else
            {
                chapter.unlockButton.gameObject.SetActive(false);  // 否則隱藏按鈕
            }
        }
    }
    /*-------------------------------------------------- Extra 檢測邏輯 --------------------------------------------------*/
    
    // 初始化 Extra 區塊
    void InitializeExtra()
    {
        // 初始化 Extra 區塊中的 UI 方塊為灰色
        for (int i = 0; i < extraItemImages.Length; i++)
        {
            if (i < extraKeyItemIndexes.Count)
            {
                // 根據道具 index 初始化圖片
                ItemData.Item item = itemData.GetItemByIndex(extraKeyItemIndexes[i]);
                extraItemImages[i].sprite = item.itemIcon;
                extraItemImages[i].color = new Color(0.5f, 0.5f, 0.5f, 1f); // 默認為灰色
            }
        }

        UpdateExtraUI();  // 開始時更新 Extra 的 UI
    }

    // 檢查 Extra 區塊的三個關鍵道具是否已經拾取
    void UpdateExtraUI()
    {
        for (int i = 0; i < extraKeyItemIndexes.Count; i++)
        {
            int keyItemIndex = extraKeyItemIndexes[i];

            if (InventorySystem.Instance.HasItem(itemData.GetItemByIndex(keyItemIndex)))
            {
                extraItemImages[i].color = Color.white;                             // 如果該道具已經拾取，顯示其圖標
                Debug.Log($"Extra key item with index {keyItemIndex} is collected.");
            }
            else
                extraItemImages[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);         // 如果道具未拾取，保持灰色
        }
    }

    private void IncrementExtraCounter(){ExtraCounter++;}
}
