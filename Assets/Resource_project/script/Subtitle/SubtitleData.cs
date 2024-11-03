using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SubtitleData", menuName = "ScriptableObjects/SubtitleData", order = 1)]
public class SubtitleData : ScriptableObject
{
    public static SubtitleData Instance; // 靜態實例

    [System.Serializable]
    public class Subtitle
    {
        public string text;
        public float displayDuration = 1.5f; // 每段字幕顯示的時間
    }

    [System.Serializable]
    public class SubtitleGroup
    {
        public string triggerName; // 用於識別字幕組的物件名稱
        public List<Subtitle> subtitles = new List<Subtitle>(); // 字幕集合
        public bool hasBeenTriggered; // 用於檢查該字幕組是否已被觸發過
    }

    public List<SubtitleGroup> subtitleGroups = new List<SubtitleGroup>();

    // 根據物件名稱獲取特定字幕組
    public SubtitleGroup GetSubtitleGroupByName(string name)
    {
        return subtitleGroups.Find(group => group.triggerName == name);
    }

    // 初始化時將靜態實例設置為當前對象，並將所有的 hasBeenTriggered 設為 false
    private void OnEnable()
    {
        Instance = this;  // 設置靜態實例
        ResetTriggerStates();
    }

    // 初始化，將所有的 hasBeenTriggered 設為 false
    public void ResetTriggerStates()
    {
        foreach (var group in subtitleGroups)
        {
            group.hasBeenTriggered = false; // 將每個字幕組的觸發狀態初始化為 false
        }
    }
}
