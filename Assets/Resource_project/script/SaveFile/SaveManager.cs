using UnityEngine;
using System.IO;
using System;

public static class SaveManager
{
    // 指定存檔欄位，欄位自行設定
    public static void SavePlayer(Player player, int slotNumber)
    {
        TeleportManager teleportManager = GameObject.FindObjectOfType<TeleportManager>();
        if (teleportManager == null)
        {
            Debug.LogError("TeleportManager not found. Save failed.");
            return;
        }

        PlayerData data = new PlayerData(player, teleportManager);
        string json = JsonUtility.ToJson(data);
        string path = GetSavePath(slotNumber); // 動態生成存檔路徑

        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"存檔已建立: {path} - 時間: {data.saveTime}");
        }
        catch (Exception e)
        {
            Debug.LogError("存檔失敗: " + e.Message);
        }
    }

    public static PlayerData LoadPlayer(int slotNumber)
    {
        string path = GetSavePath(slotNumber); // 根據槽位選擇讀取的文件
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError("讀取存檔失敗: " + e.Message);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("存檔不存在，使用選定編號開始新遊戲" + path);
            return null;
        }
    }

 

    // 根據數字生成不同的存檔欄位
    private static string GetSavePath(int slotNumber)
    {
        return Application.persistentDataPath + $"/player_slot{slotNumber}.json";
    }


}
