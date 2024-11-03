using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Flower;
using UnityEngine.UI;
using Unity.VisualScripting;

[System.Serializable]
public class Question
{
    public GameObject questionObject;
    public static int idCounter;
    public int Index;
    public TMP_InputField inputField;
    public string correctAnswer;
    
    public Question()
    {
        Index = idCounter++;
    }
}

public class LectureRoom : MonoBehaviour
{
    public List<Question> questions;
    private int currentProgress = 0;
    private int errorCount = 0;
    
    //是否結束
    public static bool MicIsEnd;

    public CanvasGroup backgroundCanvasGroup; // 用於控制透明度


    //InterationSystem 用來關閉此系統
    InteractionSystem interactionSystem;
    FlowerSystem fs;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)){
            fs.Next();
        }
    }
    private void OnEnable()
    {
        HideAllQuestions();
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        StartLectureRoom();
        StartCoroutine(FadeInBackground(0.5f)); // 開始淡入背景
    }

    public void StartLectureRoom()
    {

        interactionSystem = FindObjectOfType<InteractionSystem>();
        if (interactionSystem == null ) 
        {
            Debug.Log("InteractionSystem can't find");
        }
        ClearAllInputFields();
        StartCoroutine(HandleProgress());
    }
    // 背景淡入效果
    private IEnumerator FadeInBackground(float duration)
    {
        float time = 0f;
        backgroundCanvasGroup.alpha = 0f;  // 開始時透明

        while (time < duration)
        {
            time += Time.deltaTime;
            backgroundCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time / duration);  // 緩慢增大透明度
            yield return null;
        }

        backgroundCanvasGroup.alpha = 1f;  // 最終設定為完全顯示
    }

    private IEnumerator HandleProgress()
    {
        while (true)
        {
            switch (currentProgress)
                {

                    case 0:
                        fs.SetupDialog("PlotDialogPrefab");
                        fs.ReadTextFromResource("LectureRoom/LectureRoom");
                        yield return new WaitUntil(() => fs.isCompleted);
                        fs.SetupDialog("PlotDialogPrefab");
                        fs.SetTextList(new List<string> { "第一題[w] [remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        ShowQuestion(currentProgress);
                        break;

                    case 1:
                        fs.SetupDialog("PlayerDialogPrefab");
                        fs.SetTextList(new List<string> { "第二題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        ShowQuestion(currentProgress);
                        break;

                    case 2:
                        fs.SetupDialog("PlayerDialogPrefab");
                        fs.SetTextList(new List<string> { "第三題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        ShowQuestion(currentProgress);
                        break;
                    case 3:
                        fs.SetupDialog("PlayerDialogPrefab");
                        fs.SetTextList(new List<string> { "第四題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        ShowQuestion(currentProgress);
                        break;
                    case 4:
                        fs.SetupDialog("PlayerDialogPrefab");
                        fs.SetTextList(new List<string> { "第五題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        ShowQuestion(currentProgress);
                        break;
                    case 5:
                        fs.SetupDialog("PlayerDialogPrefab");
                        fs.SetTextList(new List<string> { "第六題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        ShowQuestion(currentProgress);
                        break;
                    case 6:
                        fs.SetupDialog("PlayerDialogPrefab");
                        fs.SetTextList(new List<string> { "第七題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        ShowQuestion(currentProgress);
                        break;
                    case 7:
                        Debug.Log("All questions completed or error limit reached!");
                        Debug.Log("Errorcunt" + errorCount);
                        if (errorCount > 0)
                        {
                            StartCoroutine(ShowEndMessage());
                            break;
                        }
                        else
                        {
                            StartCoroutine(ShowWinMessage());
                            break;
                        }
                }

            //等待輸入框輸入完成以及對話框結束
            if (currentProgress == 7 || errorCount >= 3)
            {
                break;
            }
            if (currentProgress < questions.Count)
            {
                // 等待輸入框輸入完成以及對話框結束
                yield return new WaitUntil(() => !questions[currentProgress].questionObject.activeSelf && fs.isCompleted);
                currentProgress++;
            }
        }
    }


    public void ShowQuestion(int progress)
    {
        if (progress < 0 || progress >= questions.Count)
        {
            Debug.LogError("Progress value is out of range.");
            return;
        }
        
        // 清除過去添加的監聽器
        questions[progress].inputField.onEndEdit.RemoveListener(OnInputFieldSubmit);
        // 設置當前問題輸入框被啟用
        questions[progress].questionObject.SetActive(true);

        Debug.Log("Current Progress: " + progress);

        // 設置輸入框監聽器
        questions[progress].inputField.onEndEdit.AddListener(OnInputFieldSubmit);

        // 啟用輸入框
        questions[progress].inputField.Select();
        questions[progress].inputField.ActivateInputField();
    }

    private void HideCurrentQuestion()
    {
        questions[currentProgress].questionObject.SetActive(false);
        questions[currentProgress].inputField.onSubmit.RemoveListener(OnInputFieldSubmit);
    }

    private void HideAllQuestions()
    {
        foreach (var question in questions)
        {
            question.questionObject.SetActive(false);
        }
    }

    private void OnInputFieldSubmit(string input)
    {
        // 只在Enter時觸發
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("User submitted input: " + input);

            Question currentQuestion = questions[currentProgress];

            if (input == currentQuestion.correctAnswer)
            {
                Debug.Log("Correct answer!");

                HideCurrentQuestion();
            }
            else
            {
                Debug.Log("Incorrect answer!");
                errorCount++;
                HideCurrentQuestion();

                //錯誤三次
                if (errorCount < 3)
                {
                    //顯示錯誤訊息
                    StartCoroutine(ShowErrorMessage());
                    Debug.Log("Ready for next question.");
                }
                else if (errorCount == 3)
                {
                    StartCoroutine(ShowEndMessage());
                  
                }
            }
        }
    }


    private IEnumerator ShowErrorMessage()
    {
        fs.SetupDialog("EnviromentDialogPrefab");
        fs.SetTextList(new List<string> { "嗯.....[w]好像不太對喔....[w]但是沒關係，我們還有接下來的題目[w][remove_dialog]" });


        yield return new WaitUntil(() => fs.isCompleted);
    }


    private IEnumerator ShowEndMessage()
    {
        fs.SetupDialog("EnviromentDialogPrefab");
        fs.SetTextList(new List<string> { "錯的有點多喔，妳真的有在狀況內嗎?[w]又或者妳還是不知道呢?[w] [remove_dialog]" });
        yield return new WaitUntil(() => fs.isCompleted);

        //關閉當前畫面
        interactionSystem.ExamineObject();
    }
    //清除所有問題的輸入框內容
    public void ClearAllInputFields()
    {
        foreach (var question in questions)
        {
            if (question.inputField != null)
            {
                question.inputField.text = ""; // 清除輸入框內容
            }
        }

        // 重製進度
        currentProgress = 0;
        errorCount = 0;
    }

    //完成後顯示
    public IEnumerator ShowWinMessage() 
    {
        fs.SetupDialog("EnviromentDialogPrefab");
        fs.SetTextList(new List<string> { "@{./!,&.\"$$'\">%~[w] 頭上的廣播傳出了異樣的雜音[w][play_end]" });
        yield return new WaitUntil(() => fs.isCompleted);
        yield return new WaitForSeconds(1.5f);
        CloseInteration();
        //拿到道具跟加到圖鑑
        Item item = GameObject.Find("Mic").GetComponent<Item>();
        item.PickUpItem();
    }

    //將互動關閉
    public void CloseInteration()
    {
        MicIsEnd = true;
        GameObject Mic = GameObject.Find("Mic");

        if (Mic != null ) 
        {
          
            Item item = Mic.GetComponent<Item>();
            if (item != null)
            {
                item.interactionType = Item.InteractionType.NONE;
            }

            // 確保 InteractionSystem 存在
            if (interactionSystem != null)
            {
                // 調用 ExamineObject 方法
                interactionSystem.ExamineObject();
            }
            else
            {
                Debug.LogError("Player does not have InteractionSystem component");
            }
        }
    }
}
