using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public struct DialogueEntry
    {
        public string dialogueName;
        public int dialogueIndex;  // 自增的索引值
        public bool SaidByPlayer;
        public List<string> dialogueLines;

    }

    public List<DialogueEntry> dialogues = new List<DialogueEntry>();

    // 在 Inspector 中驗證並自動更新對話索引
    private void OnValidate()
    {
        // 自動為每個 DialogueEntry 分配遞增的索引
        for (int i = 0; i < dialogues.Count; i++)
        {
            // 獲取列表中的結構體
            DialogueEntry entry = dialogues[i];

            // 修改結構體字段
            entry.dialogueIndex = i;

            // 將修改後的結構體放回列表
            dialogues[i] = entry;
        }
    }

    // 通過索引獲取對話
    public List<string> GetDialogueByIndex(int index)
    {
        foreach (var dialogue in dialogues)
        {
            if (dialogue.dialogueIndex == index)
            {
                return dialogue.dialogueLines;
            }
        }
        Debug.LogError("Dialogue index not found");
        return null;
    }

    // 通過名字獲取對話
    public List<string> GetDialogueByName(string name)
    {
        foreach (var dialogue in dialogues)
        {
            if (dialogue.dialogueName == name)
            {
                return dialogue.dialogueLines;
            }
        }
        Debug.LogError("Dialogue name not found");
        return null;
    }
}
