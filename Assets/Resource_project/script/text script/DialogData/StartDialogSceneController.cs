using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;

public class StartDialogSceneController : MonoBehaviour
{
    private FlowerSystem fs;
    public int dialogueindex;
    public GameObject toturrial;

    private void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        // 確保相關部分完成
        StartCoroutine(WaitForInitializationAndTriggerDialogue());
    }

    private IEnumerator WaitForInitializationAndTriggerDialogue()
    {
        yield return new WaitUntil(() => DialogueManager.Instance != null && DialogueManager.Instance.isInitialized);

        // Ensure that the save data has been applied
        yield return new WaitForEndOfFrame();



        // 觸發對話
        TriggerDialogue(dialogueindex);
    }

    private void TriggerDialogue(int dialogueIndex)
    {
        // 取的對應的 DialogueEntry 並檢查 IsInteration 值
        StageEnviromentDialog dialogueData = DialogueManager.Instance.StageEnviromentDialog;
        var dialogueEntry = dialogueData.dialogues.Find(d => d.dialogueIndex == dialogueIndex);


        if (dialogueEntry.IsInteration)
        {
            Debug.Log($"Dialogue with index {dialogueIndex} has already been interacted with. Skipping dialogue.");
            return;
        }
        // 檢查目前是否有任何對話在運行
        if (fs.isCompleted)
        {
            List<string> dialogueLines = dialogueData.GetDialogueByIndex(dialogueIndex);
            if (dialogueLines != null)
            {
                fs.SetupDialog("PlayerDialogPrefab");
                fs.SetTextList(dialogueLines);

                // 啟動協成更改對話的IsInteration
                StartCoroutine(WaitForDialogueCompletion(dialogueIndex));
            }
        }
    }

    private IEnumerator WaitForDialogueCompletion(int dialogueIndex)
    {
        // 等待对话结束 (fs.isCompleted 变为 true)
        yield return new WaitUntil(() => fs.isCompleted);

        toturrial.SetActive(true);

        // 获取对应的 DialogueEntry 并将 IsInteration 设置为 true
        StageEnviromentDialog dialogueData = DialogueManager.Instance.StageEnviromentDialog;

        // 使用 for 循环代替 foreach 来修改列表中的元素
        for (int i = 0; i < dialogueData.dialogues.Count; i++)
        {
            if (dialogueData.dialogues[i].dialogueIndex == dialogueIndex)
            {
                // 修改对应的 DialogueEntry
                var dialogue = dialogueData.dialogues[i];
                dialogue.IsInteration = true;

                // 将修改后的对象重新赋值回列表中
                dialogueData.dialogues[i] = dialogue;
                break;
            }
        }
    }
}
