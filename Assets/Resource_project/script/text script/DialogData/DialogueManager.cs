using UnityEngine;
using System.Collections.Generic;
using Flower;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    // 單例實例
    public static DialogueManager Instance { get; private set; }

    // 對話數據
    public DialogueData dialogueData;
    public StageEnviromentDialog StageEnviromentDialog;

    private Dictionary<string, List<string>> dialogues = new Dictionary<string, List<string>>();

    [HideInInspector]
    public bool isInitialized = false;

    private void Awake()
    {
        // 單例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保證在場景切換時不會被銷毀
        }
        else
        {
            Destroy(gameObject); // 銷毀新的實例
        }

        StartCoroutine(InitializeDialogueManager());
    }

    private void Start()
    {
        RegisterDialogues(dialogueData); // 註冊對話數據
    }

    //加載物件
    private IEnumerator InitializeDialogueManager()
    {
        yield return new WaitForEndOfFrame(); // 等待所有物件加載完畢
        RegisterDialogues(dialogueData);
        isInitialized = true;
    }

    // 註冊單個對話
    public void RegisterDialogue(string dialogueName, List<string> dialogueLines)
    {
        if (!dialogues.ContainsKey(dialogueName))
        {
            dialogues.Add(dialogueName, dialogueLines);
        }
    }

    // 批量註冊對話
    public void RegisterDialogues(DialogueData dialogueData)
    {
        foreach (var entry in dialogueData.dialogues)
        {
            RegisterDialogue(entry.dialogueName, entry.dialogueLines);
        }
    }

    // 開始對話
    public void StartDialogue(string dialogueName, FlowerSystem fs)
    {
        if (dialogues.TryGetValue(dialogueName, out List<string> dialogueLines))
        {
            fs.SetupDialog();
            fs.SetTextList(dialogueLines);
        }
        else
        {
            Debug.LogError("Dialogue not found: " + dialogueName);
        }
    }
}
