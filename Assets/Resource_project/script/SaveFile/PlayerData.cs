using System.Collections.Generic;
using System;
using UnityEditor.Rendering;
using UnityEngine;
using System.Linq;

[Serializable]
public class PlayerData
{
    public int SceneIndex;
    public float[] position;
    public List<ItemState> itemStates; // 保存 Item 的狀態
    public List<DialogueState> dialogueStates; // 保存 Dialogue 的狀態
    public int CameraConfinerIndex;

    public int NarcissusUseCounter;
    public bool lectureRoomCompleted; // 保存演講廳的進度
    public bool counselingRoomCompleted; // 保存輔導室的進度
    public bool ShadowTrigger;

    public bool bookHasItem;//保存第三段書櫃的進度

    public List<EncyclopediaState> encyclopediaStates;  // 使用新的 List 來存儲圖鑑系統的進度
    public List<SubtitleState> subtitleStates; // 保存字幕系統的進度
    public string saveTime; // 存檔時間



    public PlayerData(Player player , TeleportManager teleportManager)
    {
        saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // 將 DateTime 轉換為字串
        SceneIndex = player.GetSceneIndex();
        position = new float[2];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;

        //水仙花的次數
        NarcissusUseCounter = Narcissus.NarcissusUseCount;

        //演講廳的進度
        lectureRoomCompleted = LectureRoom.MicIsEnd;

        //輔導室的進度
        counselingRoomCompleted = CounselingRoomController.IsEnd;

        //陰影的保存狀態，場景2專屬
        if(SceneIndex == 4)
        {
            ShadowTrigger = CorridorShadow.ShadowTrigger;
        }
       
        //初始化並保存InventorySystem的道具狀態
        itemStates = new List<ItemState>();
        int[] excludeIndices = { 9, 10, 11, 12, 13, 14, 15, 16 };

        foreach (var item in InventorySystem.Instance.items)
        {
            // 檢查當前item的index是否在排除列表中
            if (!excludeIndices.Contains(item.index))
            {
                itemStates.Add(new ItemState(item.index, item.itemName, item.IsInteration));
            }
        }

        //初始化並保存所有StageEnviromentDialog的互動狀態
        dialogueStates = new List<DialogueState>();
        foreach (var dialogue in StageEnviromentDialog.Instance.dialogues)
        {
            dialogueStates.Add(new DialogueState(dialogue.dialogueIndex, dialogue.dialogueName, dialogue.IsInteration));
        }

        // 保存字幕系統的狀態
        subtitleStates = new List<SubtitleState>();
        foreach (var subtitleGroup in SubtitleData.Instance.subtitleGroups)
        {
            // 保存每個字幕組的狀態
            var subtitleState = new SubtitleState(subtitleGroup.triggerName, subtitleGroup.hasBeenTriggered);
            foreach (var subtitle in subtitleGroup.subtitles)
            {
                // 保存字幕內容
                subtitleState.subtitles.Add(new SubtitleContent(subtitle.text, subtitle.displayDuration));
            }
            subtitleStates.Add(subtitleState);
        }


        // 保存目前的 CameraConfinerIndex
        CameraConfinerIndex = teleportManager.GetCurrentConfinerIndex();
        
        // 檢查 `bookcase_with_item` 是否存在並保存其狀態
        if(SceneIndex == 7)
        {
            GameObject bookWithItemObject = GameObject.Find("bookcase_with_item");
            if (bookWithItemObject != null)
            {
                BookCase bookWithItem = bookWithItemObject.GetComponent<BookCase>();
                bookHasItem = bookWithItem.HasItem; // 記錄物品的狀態
            }
            else
            {
                Debug.Log("未找到 `BookCase` 組件！");
            }
        }
       


        // 初始化并保存所有章节和道具状态
        encyclopediaStates = new List<EncyclopediaState>();
        foreach (var chapter in EncyclopediaUI.Instance.chapters)
        {
            var state = new EncyclopediaState(
                chapter.chapterName,
                EncyclopediaUI.EncyclopediaProgress.unlockedChapters.ContainsKey(chapter.chapterName) && EncyclopediaUI.EncyclopediaProgress.unlockedChapters[chapter.chapterName]
            );

            foreach (var item in chapter.items)
            {
                bool isCollected = EncyclopediaUI.EncyclopediaProgress.collectedItems.ContainsKey(item.itemName) && EncyclopediaUI.EncyclopediaProgress.collectedItems[item.itemName];
                state.collectedItems.Add(new EncyclopediaCollectedItemState(item.itemName, isCollected));
            }

            encyclopediaStates.Add(state);
        }

    }
}

[Serializable]
public class ItemState
{
    public int index;
    public string itemName;
    public bool IsInteration;

    public ItemState(int index, string itemName, bool isInteration)
    {
        this.index = index;
        this.itemName = itemName;
        this.IsInteration = isInteration;
    }
}


[Serializable]
public class DialogueState
{
    public int index;
    public string dialogueName;
    public bool IsInteration;

    public DialogueState(int index, string dialogueName, bool isInteration)
    {
        this.index = index;
        this.dialogueName = dialogueName;
        this.IsInteration = isInteration;
    }
}

[Serializable]
public class EncyclopediaState
{
    public string chapterName;  // 保存章節名稱
    public bool isUnlocked;     // 保存章節是否解鎖
    public List<EncyclopediaCollectedItemState> collectedItems;  // 保存該章節已收集的道具狀態

    public EncyclopediaState(string chapterName, bool isUnlocked)
    {
        this.chapterName = chapterName;
        this.isUnlocked = isUnlocked;
        this.collectedItems = new List<EncyclopediaCollectedItemState>();
    }
}

[Serializable]
public class EncyclopediaCollectedItemState
{
    public string itemName;  // 道具名稱
    public bool isCollected; // 道具是否被收集

    public EncyclopediaCollectedItemState(string itemName, bool isCollected)
    {
        this.itemName = itemName;
        this.isCollected = isCollected;
    }
}

[Serializable]
public class SubtitleState
{
    public string triggerName; // 保存字幕組的名稱
    public bool hasBeenTriggered; // 保存該字幕組是否被觸發
    public List<SubtitleContent> subtitles; // 保存字幕內容

    public SubtitleState(string triggerName, bool hasBeenTriggered)
    {
        this.triggerName = triggerName;
        this.hasBeenTriggered = hasBeenTriggered;
        this.subtitles = new List<SubtitleContent>();
    }
}

[Serializable]
public class SubtitleContent
{
    public string text; // 字幕文本
    public float displayDuration; // 顯示持續時間

    public SubtitleContent(string text, float displayDuration)
    {
        this.text = text;
        this.displayDuration = displayDuration;
    }
}

