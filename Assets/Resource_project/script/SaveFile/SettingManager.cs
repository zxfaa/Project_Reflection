using UnityEngine;
using System.IO;
using System;

public static class SettingsManager
{
    private const string SettingsFileName = "game_settings.json";

    // 所有設置都集中在這裡
    public static float MusicVolume { get; private set; }//BGB音量
    public static float GameVoulume {  get; private set; }//遊戲內音量
    public static bool IsMuted { get; private set; }//是否被靜音
    public static float TextSpeed { get; private set; }//文字速度
    public static float DialogAlpha { get; private set; }//對話框透明度

    // 初始化方法
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Start()
    {
        LoadSettings();
    }

    public static void SaveSettings()
    {
        var data = new SettingData
        {
            MusicVolume = MusicVolume,
            GameVolume = GameVoulume,
            IsMuted = IsMuted,
            TextSpeed = TextSpeed,
            DialogAlpha = DialogAlpha
        };
        SaveSettingsToFile(data);
    }

    private static void LoadSettings()
    {
        string path = Path.Combine(Application.persistentDataPath, SettingsFileName);
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                SettingData data = JsonUtility.FromJson<SettingData>(json);
                ApplySettings(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"加載設定失敗：{e.Message}");
                UseDefaultSettings();
            }
        }
        else
        {
            Debug.Log("未找到設定檔案，使用默認設定");
            UseDefaultSettings();
        }
    }

    private static void UseDefaultSettings()
    {
        ApplySettings(new SettingData());
    }

    private static void ApplySettings(SettingData data)
    {
        MusicVolume = data.MusicVolume;
        GameVoulume = data.GameVolume;
        IsMuted = data.IsMuted;
        TextSpeed = data.TextSpeed;
        DialogAlpha = data.DialogAlpha;
        Debug.Log($"設定已更新 - 音量: {MusicVolume},遊戲音量{GameVoulume},音樂狀態{IsMuted}, 文字速度: {TextSpeed}, 對話框透明度: {DialogAlpha}");
    }

    private static void SaveSettingsToFile(SettingData data)
    {
        string path = Path.Combine(Application.persistentDataPath, SettingsFileName);
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log("設定已保存");
        }
        catch (Exception e)
        {
            Debug.LogError($"保存設定失敗：{e.Message}");
        }
    }

    // 提供更新單個設置的方法
    public static void UpdateMusicVolume(float value)
    {
        MusicVolume = value;
        SaveSettings();
    }
    public static void UpdateGameVolume(float value)
    {
        GameVoulume = value;
        SaveSettings();
    }
    public static void UpdateIsMuted(bool muted)
    {
        IsMuted = muted;  
        SaveSettings();
    }
    public static void UpdateTextSpeed(float value)
    {
        TextSpeed = value;
        SaveSettings();
    }

    public static void UpdateDialogAlpha(float value)
    {
        DialogAlpha = value;
        SaveSettings();
    }
}