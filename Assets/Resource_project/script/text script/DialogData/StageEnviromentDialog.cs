using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnviromentDialogueData", menuName = "Dialogue/Enviroment Dialogue Data")]
public class StageEnviromentDialog : ScriptableObject
{
    public static StageEnviromentDialog Instance; // 靜態實例

    [System.Serializable]
    public struct DialogueEntry
    {
        public string dialogueName;
        public bool IsInteration;
        public int dialogueIndex;  // 自增的索引值
        public bool SaidByPlayer;
        public List<string> dialogueLines;
    }

    public List<DialogueEntry> dialogues = new List<DialogueEntry>();

    private void OnEnable()
    {
        Instance = this;
        ResetInteractionStates();
    }

    private void OnValidate()
    {
        for (int i = 0; i < dialogues.Count; i++)
        {
            DialogueEntry entry = dialogues[i];
            entry.dialogueIndex = i;
            dialogues[i] = entry;
        }
    }

    // 將所有的 IsInteration 改為 false
    private void ResetInteractionStates()
    {
        for (int i = 0; i < dialogues.Count; i++)
        {
            DialogueEntry entry = dialogues[i];
            entry.IsInteration = false;
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
