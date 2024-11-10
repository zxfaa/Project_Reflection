using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class SaveSystemSecond : MonoBehaviour
{
    public static SaveSystemSecond Instance { get; private set; }
    [SerializeField] private GameObject playerPrefab;
    private Player currentPlayer;

    public static int currentPlayerNumber; // 全局唯一的遊玩編號
    private int currentSlotNumber; // 存檔編號
    public TextMeshProUGUI[] saveSlotTexts;  // 顯示存檔時間

    private Vector2 loadedPosition;
    private int loadedSceneIndex;
    private bool isLoadingFromSave = false;
   
    LevelLoader levelLoader;

    public CinemachineVirtualCamera virtualCamera;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        RestoreSaveTime();
        
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SavePlayer(int slotNumber)
    {
        currentPlayer = FindObjectOfType<Player>();
        if (currentPlayer != null)
        {
            // 儲存玩家狀態
            PlayerData data = new PlayerData(currentPlayer, FindObjectOfType<TeleportManager>());
            SaveManager.SavePlayer(currentPlayer, slotNumber); // 傳遞存檔槽編號
        }
        else
        {
            Debug.LogWarning("No player to save.");
        }
    }

    public void LoadPlayer(int slotNumber)
    {
        PlayerData data = SaveManager.LoadPlayer(slotNumber); // 根據槽位選擇存檔
        levelLoader = FindObjectOfType<LevelLoader>();

        if (data != null)
        {
            // 載入已存在的存檔
            loadedPosition = new Vector2(data.position[0], data.position[1]);
            loadedSceneIndex = data.SceneIndex;
        }
        else
        {
            loadedSceneIndex =1;
        }

        // 無論是否找到存檔，都更新當前玩家編號
        currentSlotNumber = slotNumber;
        currentPlayerNumber = currentSlotNumber;
        Debug.Log($"根據存檔編號{currentPlayerNumber+1}開始遊戲。");

        // 標記為從存檔加載
        isLoadingFromSave = false;

        if (levelLoader != null)
        {
            levelLoader.LoadLevel(loadedSceneIndex);
        }
        else
        {
            Debug.LogError("LevelLoader not found.");
        }
    }
    public void StartGame(int SceneIndex)
    {
        isLoadingFromSave = false;
        levelLoader = FindObjectOfType<LevelLoader>();

        for (int i = 0; i < 3; i++)
        {
            PlayerData data = SaveManager.LoadPlayer(i); // 根據槽位選擇存檔
            if (data == null)
            {
                currentPlayerNumber = i;
                currentSlotNumber = i;
                levelLoader.LoadLevel(SceneIndex);
                Debug.Log($"根據存檔編號{currentPlayerNumber + 1}開始遊戲。");
                return; // 找到空位後立即返回，避免繼續執行後續代碼
            }
        }

        // 如果 3 個存檔槽都已使用，則預設使用第 0 號存檔槽
        currentPlayerNumber = 0;
        currentSlotNumber = 0;
        levelLoader.LoadLevel(SceneIndex);

        Debug.Log($"所有存檔槽已滿，使用存檔編號{currentPlayerNumber + 1}開始遊戲。");
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isLoadingFromSave)
        {
            SetPlayerPosition();
            // 確保圖鑑以及對話被初始化
            StartCoroutine(WaitForDependenciesAndLoadData());
        }

    }

    private IEnumerator WaitForDependenciesAndLoadData()
    {
        //等待 EncyclopediaUI 和 StageEnviromentDialog 完成初始化
        while (EncyclopediaUI.Instance == null || !EncyclopediaUI.Instance.IsInitialized || StageEnviromentDialog.Instance == null)
        {
            yield return null;
        }

        PlayerData loadedData =  SaveManager.LoadPlayer(currentSlotNumber);

        if (isLoadingFromSave)
        {
            
            //解鎖場面上KeyObject互動
            RestoreKeyObjectStates(loadedData);
            //恢復物品到道具欄
            RestoreItemsFromPlayerData(loadedData);
            //更新物品互動狀態
            UpdateItemsInteractionStatus(loadedData);
            //回復對話框狀態
            RestoreDialogueStates(loadedData.dialogueStates);
            //回復水仙花計數器
            RestoreNarcissusState(loadedData);
            //回復字幕狀態 
            RestoreSubtitleData(loadedData);
            //演講廳
            RestoreLectureRoomState(loadedData);
            //輔導室
            RestoreCounselingRoomState(loadedData);
            // 恢復bookcase_with_item的HasItem狀態
            RestoreBookCaseState(loadedData);
            //回復圖鑑系統狀態
            RestoreEncyclopediaUI(loadedData);
            //回復陰影狀態
            RestoreShadowState(loadedData); 
            isLoadingFromSave = false;
        }
    }

    private void RestoreSaveTime()
    {
        for (int i = 0; i < 3; i++)
        {
            PlayerData data = SaveManager.LoadPlayer(i); // 加載第 i 個存檔
            if (data != null)
            {
                saveSlotTexts[i].text = $"存檔時間: {data.saveTime}";

            }
            else
            {
                saveSlotTexts[i].text = "存檔空";
            }
        }
    }

    private void RestoreItemsFromPlayerData(PlayerData playerData)
    {
        InventorySystem inventorySystem = FindObjectOfType<InventorySystem>();

        if (inventorySystem == null)
        {
            Debug.LogError("InventorySystem not found!");
            return;
        }

        // 清空当前的道具栏
        inventorySystem.items.Clear();

        // 遍历存档中的道具状态，将它们逐个恢复
        foreach (ItemState itemState in playerData.itemStates)
        {
            // 在 ItemData 中查找与存档相符的道具
            ItemData.Item itemToRestore = ItemData.Instance.items.Find(i => i.index == itemState.index && i.itemName == itemState.itemName);
            // 恢复道具的互动状态
            itemToRestore.IsInteration = itemState.IsInteration;
            // 将道具添加到道具栏
            inventorySystem.PickUp(itemToRestore);
            Debug.Log($"Restored item: {itemToRestore.itemName} with interaction status: {itemToRestore.IsInteration}");
            
        }
    }

    private void UpdateItemsInteractionStatus(PlayerData playerData)
    {
        if (playerData == null || playerData.itemStates == null)
        {
            Debug.LogError("PlayerData or itemStates is null");
            return;
        }

        // 查找场景中所有挂载了 Item 脚本的对象
        Item[] itemsInScene = FindObjectsOfType<Item>();

        foreach (Item itemInScene in itemsInScene)
        {
            if (itemInScene.itemIndices == null || itemInScene.itemIndices.Length == 0)
            {
                continue; // 跳过没有设置 Item Indices 的物体
            }

            bool allIndicesMatch = true;
            bool anyInteracted = false;

            // 检查该 Item 的所有 Indices 是否都在 PlayerData 中有匹配项，并且是否有任何一个被标记为已交互
            foreach (int itemIndex in itemInScene.itemIndices)
            {
                ItemState matchedState = playerData.itemStates.Find(state =>
                    state.index == itemIndex &&
                    state.itemName == itemInScene.itemData.items[itemIndex].itemName);

                if (matchedState == null)
                {
                    allIndicesMatch = false;
                    break;
                }

                if (matchedState.IsInteration)
                {
                    anyInteracted = true;
                }
            }

            // 如果所有 Indices 都匹配，并且至少有一个被标记为已交互，则禁用交互
            if (allIndicesMatch && anyInteracted)
            {
                itemInScene.interactionType = Item.InteractionType.NONE;
                itemInScene.dialogueIndex = 0;
                Debug.Log($"Item '{itemInScene.name}' interaction disabled based on saved data. All indices match and at least one interacted.");
            }
        }
    }



    private void RestoreEncyclopediaUI(PlayerData data)
    {
        if (EncyclopediaUI.Instance == null)
        {
            Debug.LogError("EncyclopediaUI.Instance is null");
            return;
        }

        // 清空现有的进度数据
        EncyclopediaUI.EncyclopediaProgress.unlockedChapters.Clear();
        EncyclopediaUI.EncyclopediaProgress.collectedItems.Clear();

        // 恢复章节和物品状态
        foreach (var chapterState in data.encyclopediaStates)
        {
            // 恢复章节解锁状态
            EncyclopediaUI.EncyclopediaProgress.unlockedChapters[chapterState.chapterName] = chapterState.isUnlocked;

            // 恢复物品收集状态
            foreach (var itemState in chapterState.collectedItems)
            {
                EncyclopediaUI.EncyclopediaProgress.collectedItems[itemState.itemName] = itemState.isCollected;
            }
        }

        // 更新UI
        EncyclopediaUI.Instance.RestoreItemImages();

        // 恢复章节按钮状态
        foreach (var chapter in EncyclopediaUI.Instance.chapters)
        {
            bool isUnlocked = EncyclopediaUI.EncyclopediaProgress.unlockedChapters.ContainsKey(chapter.chapterName) &&
                              EncyclopediaUI.EncyclopediaProgress.unlockedChapters[chapter.chapterName];

            chapter.unlockButton.gameObject.SetActive(isUnlocked);
            chapter.unlockButton.interactable = isUnlocked;

            
        }

        Debug.Log("EncyclopediaUI state restored successfully");
    }



    private void RestoreDialogueStates(List<DialogueState> dialogueStates)
    {
        foreach (var state in dialogueStates)
        {
            for (int i = 0; i < StageEnviromentDialog.Instance.dialogues.Count; i++)
            {
                if (StageEnviromentDialog.Instance.dialogues[i].dialogueIndex == state.index &&
                    StageEnviromentDialog.Instance.dialogues[i].dialogueName == state.dialogueName)
                {
                    var dialogue = StageEnviromentDialog.Instance.dialogues[i];
                    dialogue.IsInteration = state.IsInteration;
                    StageEnviromentDialog.Instance.dialogues[i] = dialogue;
                    break;
                }
            }
        }
    }

    private void RestoreSubtitleData(PlayerData loadedData)
    {
        if (SubtitleData.Instance == null)
        {
            Debug.LogError("SubtitleData.Instance is null.");
            return;
        }

        foreach (var subtitleState in loadedData.subtitleStates)
        {
            var group = SubtitleData.Instance.GetSubtitleGroupByName(subtitleState.triggerName);
            if (group != null)
            {
                group.hasBeenTriggered = subtitleState.hasBeenTriggered;
                for (int i = 0; i < subtitleState.subtitles.Count; i++)
                {
                    if (i < group.subtitles.Count)
                    {
                        group.subtitles[i].text = subtitleState.subtitles[i].text;
                        group.subtitles[i].displayDuration = subtitleState.subtitles[i].displayDuration;
                    }
                }
            }
        }

        Debug.Log("Subtitle data restored successfully.");
    }


    private void RestoreKeyObjectStates(PlayerData loadedData)
    {
        if (loadedData == null || loadedData.itemStates == null)
        {
            Debug.LogError("Loaded data or item states are null");
            return;
        }

        foreach (var itemState in loadedData.itemStates)
        {
            if (itemState.IsInteration)
            {
                // 查找場景中所有的 Item 組件
                Item[] sceneItems = FindObjectsOfType<Item>();
                foreach (Item sceneItem in sceneItems)
                {
                    // 檢查是否是關鍵物品，並且索引匹配
                    if (sceneItem.isKeyObject && sceneItem.itemIndices.Contains(itemState.index))
                    {
                        // 觸發 InteractionManager 中的解鎖邏輯
                        InteractionManager.Instance.ObjectInteracted(sceneItem.objectId);
                        Debug.Log($"Restored key object interaction: {sceneItem.objectId}");
                        break; // 找到匹配項後跳出內部循環
                    }
                }
            }
        }
    }

    private void RestoreLectureRoomState(PlayerData data)
    {
        LectureRoom.MicIsEnd = data.lectureRoomCompleted;
        if (data.lectureRoomCompleted)
        {
            GameObject mic = GameObject.Find("Mic");
            if (mic != null)
            {
                Item item = mic.GetComponent<Item>();
                if (item != null)
                {
                    item.interactionType = Item.InteractionType.NONE;
                }
            }
        }
    }
    private void RestoreCounselingRoomState(PlayerData data)
    {
        CounselingRoomController.IsEnd = data.counselingRoomCompleted;
        if (data.counselingRoomCompleted)
        {
            GameObject counselingDoor = GameObject.Find("CounselingDoor");
            if (counselingDoor != null)
            {
                Item item = counselingDoor.GetComponent<Item>();
                if (item != null)
                {
                    item.isKeyObject = true;
                }
            }
        }

    }

    private void RestoreNarcissusState(PlayerData data)
    {
        Narcissus.NarcissusUseCount = data.NarcissusUseCounter;
    }

    private void RestoreBookCaseState(PlayerData data)
    {
        // 找到場景中的bookcase_with_item物件並恢復狀態
        GameObject bookcaseObject = GameObject.Find("bookcase_with_item");
        if (bookcaseObject != null)
        {
            BookCase bookCase = bookcaseObject.GetComponent<BookCase>();
            if (bookCase != null)
            {
                bookCase.HasItem = data.bookHasItem; // 恢復HasItem狀態
                Debug.Log($"BookCase狀態已恢復：HasItem = {bookCase.HasItem}");
            }
        }
    }
    private void RestoreShadowState(PlayerData data)
    {
        CorridorShadow.ShadowTrigger = data.ShadowTrigger;
        GameObject Shadow = GameObject.Find("PlayerShadow");
        Shadow.SetActive(false);
    }

    private void SetPlayerPosition()
    {
        currentPlayer = FindObjectOfType<Player>();
        if (currentPlayer == null)
        {
            Instantiate(playerPrefab);
            currentPlayer = FindObjectOfType<Player>();
        }
        Debug.Log($"Setting player position to: {loadedPosition}");


        currentPlayer.transform.position = loadedPosition;
        currentPlayer.targetPosition = loadedPosition;
        currentPlayer.moveDirection = Vector2.zero;
        //currentPlayer.animator.SetBool("isMoving", false);

        TeleportManager teleportManager = FindObjectOfType<TeleportManager>();
        if (teleportManager != null && SaveManager.LoadPlayer(currentSlotNumber) != null)
        {
            teleportManager.SetCameraConfiner(SaveManager.LoadPlayer(currentSlotNumber).CameraConfinerIndex);
        }
    }
}