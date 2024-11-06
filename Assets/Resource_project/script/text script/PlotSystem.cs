using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Flower;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

public class PlotSystem : MonoBehaviour
{

    //AutoMod參數
    private bool isAutoMode;

    FlowerSystem fs;

    public GameObject AutoMod; // 在 Inspector 中設置引用
    public Button modeToggleButton; // auto mod按鈕

    public GameObject Exit; // 在 Inspector 中設置引用
    public Button exitButton; // exit 按鈕


    public bool ExitOrSkip; // true為Skip false為跳過 

    // 獲得 DialogClicker
    public GameObject DialogClicker;

    public float fadeInDuration = 0f; // 淡入持續時間
    public float delayBeforeFadeIn = 0f; // 延遲時間

    private CanvasGroup autoModCanvasGroup;
    private CanvasGroup exitCanvasGroup;

    public LevelLoader levelLoader;

    public Animator fadeAnimator;
    public Image fadeImage;

    void Awake()
    {
        fs = FlowerManager.Instance.CreateFlowerSystem("default", true);


        fs.RegisterCommand("close_clickArea", (List<string> _params) =>
        {
            DialogClicker.SetActive(false);
        });

        fs.RegisterCommand("end", (List<string> _params) =>
        {
           fs.StopAndReset();
        });

        fs.RegisterCommand("show_button", (List<string> _params) =>
        {
            ShowButton();
        });
        fs.RegisterCommand("italic", (List<string> _params) =>
        {
            GameObject dialogPanel = GameObject.Find("DialogPanel");
            // 找到 DialogPanel 下的 DialogText 物件
            Text dialogText = dialogPanel.transform.Find("DialogText").GetComponent<Text>();

            // 確認找到的 Text 組件不為空
            if (dialogText != null)
            {
                // 將字體樣式設置為斜體
                dialogText.fontStyle = FontStyle.Italic;
            }
        });
        fs.RegisterCommand("italic_end", (List<string> _params) =>
        {
            GameObject dialogPanel = GameObject.Find("DialogPanel");
            // 找到 DialogPanel 下的 DialogText 物件
            Text dialogText = dialogPanel.transform.Find("DialogText").GetComponent<Text>();

            // 確認找到的 Text 組件不為空
            if (dialogText != null)
            {
                // 將字體樣式設置為斜體
                dialogText.fontStyle = FontStyle.Normal;
            }
        });

        //移除對話框
        fs.RegisterCommand("remove_dialog", (List<string> _params) =>
        {
            fs.RemoveDialog();
        });

        //重製按鈕
        fs.RegisterCommand("reset_button", (List<string> _params) =>
        {
            ResetButton();
        });

        //跳轉場景
        fs.RegisterCommand("load_scene", (List<string> _params) =>
        {
            SceneManager.LoadScene(_params[0]);
        });

        fs.RegisterCommand("char_name", (List<string> _params) =>
        {
            DisplayCharacterName(_params[0]);
        });

        fs.RegisterCommand("hide_char", (List<string> _params) =>
        {
            HideCharacter();
        });

        //播放特定互動結尾
        fs.RegisterCommand("play_end", (List<string> _params) =>
        {
            // 檢查 fadeAnimator 和 fadeImage 是否存在
            if (fadeAnimator && fadeImage != null)
            {
                StartCoroutine(PlayEndAnimation());
            }
            else
            {
                // 查找場景中名為 "FadeImage" 的物件
                GameObject fadeImageObject = GameObject.Find("FadeImage");

                if (fadeImageObject != null)
                {
                    // 嘗試獲取 Animator 和 Image 組件
                    fadeAnimator = fadeImageObject.GetComponent<Animator>();
                    fadeImage = fadeImageObject.GetComponent<Image>();

                    // 如果成功獲取到這兩個組件，則繼續執行動畫
                    if (fadeAnimator != null && fadeImage != null)
                    {
                        StartCoroutine(PlayEndAnimation());
                    }
                    else
                    {
                        // 如果有組件找不到，顯示警告
                        Debug.LogWarning("FadeImage 找到，但缺少 Animator 或 Image 組件！");
                    }
                }
                else
                {
                    // 如果場景中找不到 FadeImage 對象，顯示警告
                    Debug.LogWarning("場景中找不到名為 FadeImage 的物件！");
                }
            }

        });

        #region 演講廳專用
        fs.RegisterCommand("FadeImage", (List<string> _params) =>
        {
            // 檢查參數是否足夠
            if (_params.Count < 3)
            {
                Debug.LogError("參數不足，需要 CanvasGroup 名稱、持續時間、目標透明度。");
                return;
            }

            // 尝试获取 CanvasGroup 名称、持续时间和透明度
            string canvasGroupName = _params[0];
            if (!float.TryParse(_params[1], out float duration))
            {
                Debug.LogError("無法解析時間，確保第二個參數是正常的浮點數。");
                return;
            }
            if (!float.TryParse(_params[2], out float targetAlpha))
            {
                Debug.LogError("無法解析目標透明度，請確保第二個參數是正常的浮點數。");
                return;
            }
            // 开始执行淡入/淡出协程
            LectureRoom lectureRoom = FindObjectOfType<LectureRoom>();

            StartCoroutine(lectureRoom.FadeCanvasGroup(canvasGroupName, duration, targetAlpha));
        });
        #endregion


        // 檢查 AutoMod 是否存在
        if (AutoMod == null)
        {
            Debug.LogWarning("AutoMod object not set in the Inspector!");
        }

        // 檢查 Exit 是否存在
        if (Exit == null)
        {
            Debug.LogWarning("Exit object not set in the Inspector!");
        }

        // 將觸發區域關閉觸碰
        if (DialogClicker != null)
        {
            DialogClicker.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DialogClicker object not set in the Inspector!");
        }

        // 檢查按鈕是否存在
        if (modeToggleButton == null)
        {
            Debug.LogWarning("Mode toggle button not set in the Inspector!");
        }
        else
        {
            // 初始化按鈕事件
            modeToggleButton.onClick.AddListener(ToggleAutoMode);
        }

        if (exitButton == null)
        {
            Debug.LogWarning("Exit button not set in the Inspector!");
        }
        else
        {
            // 初始化按鈕事件
            exitButton.onClick.AddListener(ExitPlot);
        }

        // 初始化 AutoMod
        if (AutoMod != null)
        {
            AutoMod.SetActive(false); // 初始時隱藏對象
            autoModCanvasGroup = AutoMod.GetComponent<CanvasGroup>();
            if (autoModCanvasGroup == null)
            {
                autoModCanvasGroup = AutoMod.AddComponent<CanvasGroup>();
            }
            autoModCanvasGroup.alpha = 0; // 初始透明度為0
        }

        if (Exit != null)
        {
            Exit.SetActive(false); // 初始時隱藏對象
            exitCanvasGroup = Exit.GetComponent<CanvasGroup>();
            if (exitCanvasGroup == null)
            {
                exitCanvasGroup = Exit.AddComponent<CanvasGroup>();
            }
            exitCanvasGroup.alpha = 0; // 初始透明度為0
        }

        // 更新按鈕顏色
        UpdateButton();



    }
    private void Start()
    {
        isAutoMode = false;

    }

    private void Update()
    {
        if (!fs.isCompleted)
        {
            DialogClicker.SetActive(true);
        }
        if (fs.isCompleted)
        {
            DialogClicker.SetActive(false);
        }
    }

    public Text FindCharacterTextComponent()
    {
        // 查找场景中的 "Character" 物件
        GameObject characterObject = GameObject.Find("Charactor");

        if (characterObject != null)
        {
            // 查找 "CharactorText" 子物件
            Transform textTransform = characterObject.transform.Find("CharactorText");

            if (textTransform != null)
            {
                // 获取 "CharactorText" 物件的 Text 组件
                Text characterText = textTransform.GetComponent<Text>();

                if (characterText != null)
                {
                    return characterText; // 返回 Text 组件
                }
                else
                {
                    Debug.LogError("CharactorText 上没有找到 Text 组件！");
                    return null;
                }
            }
            else
            {
                Debug.LogError("Character 下找不到 CharactorText 子物件！");
                return null;
            }
        }
        else
        {
            Debug.LogError("场景中没有找到名为 Character 的物件！");
            return null;
        }
    }

    // 设置 CanvasGroup 的淡入效果
    public IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float fadeInTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInTime)
        {
            // 逐渐增加 alpha 值
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeInTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最后的 alpha 为 1
        canvasGroup.alpha = 1;
    }
    public void HideCharacter()
    {
        // 找到 Text 组件
        Text characterText = FindCharacterTextComponent();

        if (characterText != null)
        {
            // 获取 Character 的 CanvasGroup 并进行淡出
            CanvasGroup canvasGroup = characterText.GetComponentInParent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // 启动淡出协程，时间设为 0.2 秒
                StartCoroutine(FadeOutCanvasGroup(canvasGroup, 0.2f));
            }
            else
            {
                Debug.LogError("没有找到 CanvasGroup 组件！");
            }
        }
    }

    // 设置 CanvasGroup 的淡出效果（将 alpha 设为 0）
    public IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroup, float fadeOutTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutTime)
        {
            // 逐渐减少 alpha 值
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最后的 alpha 为 0
        canvasGroup.alpha = 0;
    }


    // 设置角色名称到 Text 组件
    public void DisplayCharacterName(string characterName)
    {
        // 找到 Text 组件
        Text characterText = FindCharacterTextComponent();

        if (characterText != null)
        {
            // 将角色名称设置到 Text 组件上
            characterText.text = characterName;

            // 获取 Character 的 CanvasGroup 并进行淡入
            CanvasGroup canvasGroup = characterText.GetComponentInParent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // 启动淡入协程，时间设为 0.2 秒
                StartCoroutine(FadeInCanvasGroup(canvasGroup, 0.2f));
            }
            else
            {
                Debug.LogError("没有找到 CanvasGroup 组件！");
            }
        }
    }
    private IEnumerator PlayEndAnimation()
    {
        // 鎖定角色移動
        FindObjectOfType<Player>().LockMovement(true);

        // 使 fadeImage 顯示
        fadeImage.gameObject.SetActive(true);

        // 觸發 "FadeInTrigger" 動畫
        fadeAnimator.SetTrigger("FadeInTrigger");

        // 等待 1.5 秒（假設這是動畫的持續時間）
        yield return new WaitForSeconds(1.5f);

        // 停止並重置 FlowerSystem
        fs.StopAndReset();

        // 觸發 "FadeOutTrigger" 動畫
        fadeAnimator.SetTrigger("FadeOutTrigger");

        // 等待動畫播放結束 (假設 FadeOut 也持續 1.5 秒)
        yield return new WaitForSeconds(1.5f);

        // 停止動畫後，將 fadeImage 隱藏
        fadeImage.gameObject.SetActive(false);

        // 解鎖角色移動
        FindObjectOfType<Player>().LockMovement(false);


    }

    public void WatchPlot(int PlotIndex)
    {
        //啟動協程加載對話框
        
        fs.SetupUIStage("default", "DefaultUIStagePrefab", 8 );
        StartCoroutine(WaitForDialogToDisappearAndSetupNewDialog(PlotIndex));
    }

    private IEnumerator WaitForDialogToDisappearAndSetupNewDialog(int PlotIndex)
    {
        // 等待場面上對話框消失
        while (!fs.isCompleted)
        {
            yield return null; // 每幀檢查
        }
 
        fs.SetupDialog("PlotDialogPrefab");
        ShowButton();
        // 根據PlotIndex參數讀取對應的文本
        switch (PlotIndex)
        {
          
            case 1:
                fs.ReadTextFromResource("plot/no1");
                break;
            case 2:
                fs.ReadTextFromResource("plot/no2");
                break;
            case 3:
                fs.ReadTextFromResource("plot/no3");
                break;
            case 4:
                fs.ReadTextFromResource("plot/no4");
                break;
            case 5:
                fs.ReadTextFromResource("plot/no5");
                break;
            case 6:
                fs.ReadTextFromResource("plot/no6");
                break;
            case 7:
                fs.ReadTextFromResource("plot/no7");
                break;
            case 8:
                fs.ReadTextFromResource("plot/no8");
                break;
            case 9:
                fs.ReadTextFromResource("plot/no9");
                break;
            case 10:
                fs.ReadTextFromResource("plot/extra3.5");
                break;
            case 11:
                fs.ReadTextFromResource("plot/extra7.5");
                break;
            case 12:
                fs.ReadTextFromResource("plot/extra11.5");
                break;
            default:
                Debug.Log("没有參數傳入");
                break;
        }
    }

   
    //觸發區域專用
    public void NextDialog()
    {
        fs.Next();
    }

    #region 按鈕互動
    public void ShowButton()
    {
        if (AutoMod != null)
        {
            StartCoroutine(FadeInAutoMod());
        }
        if (Exit != null)
        {
            StartCoroutine(FadeInExit());
        }
    }

    private IEnumerator FadeInAutoMod()
    {
        AutoMod.SetActive(true); // 啟用物件

        yield return new WaitForSeconds(delayBeforeFadeIn); // 延遲進入

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            autoModCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeInDuration); // 更新透明度
            yield return null;
        }

        autoModCanvasGroup.alpha = 1; // 確保最終透明度為1
    }

    private IEnumerator FadeInExit()
    {
        Exit.SetActive(true); // 啟用物件

        yield return new WaitForSeconds(delayBeforeFadeIn); // 延遲進入

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            exitCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeInDuration); // 更新透明度
            yield return null;
        }

        exitCanvasGroup.alpha = 1; // 確保最終透明度為1
    }

    private void ResetButton()
    {
        if (autoModCanvasGroup != null)
        {
            autoModCanvasGroup.alpha = 0; // 將透明度變為0
        }
        if (exitCanvasGroup != null)
        {
            exitCanvasGroup.alpha = 0; // 將透明度變為0
        }
        if (AutoMod != null)
        {
            AutoMod.SetActive(false); // 隱藏對象
        }
        if (Exit != null)
        {
            Exit.SetActive(false); // 隱藏對象
        }
    }

    private void ToggleAutoMode()
    {
        isAutoMode = !isAutoMode;
        fs.processMode = isAutoMode ? ProcessModeType.Auto : ProcessModeType.Normal;
        UpdateButton();
    }

    private void ExitPlot()
    {
        if (ExitOrSkip)
        {
            //跳過目前劇情
            fs.StopAndReset();
            if (DialogClicker != null)
            {
                DialogClicker.SetActive(false);
            }

            ResetButton();
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1; 

            levelLoader.LoadLevel(nextSceneIndex);

        }
        else
        {
            //關閉劇情頁面
            fs.StopAndReset();
            if (DialogClicker != null)
            {
                DialogClicker.SetActive(false);
            }

            ResetButton();

        }

       
    }


    private void UpdateButton()
    {
        // normalMod顏色
        ColorBlock normalColors = new ColorBlock
        {
            normalColor = Color.white,
            highlightedColor = new Color(0.9f, 0.9f, 0.9f),
            pressedColor = new Color(0.75f, 0.75f, 0.75f),
            selectedColor = Color.white,
            disabledColor = new Color(0.6f, 0.6f, 0.6f),
            colorMultiplier = 1
        };

        // autoMod顏色
        ColorBlock autoColors = new ColorBlock
        {
            normalColor = new Color(0.6f, 0.6f, 0.6f), // 淺灰色
            highlightedColor = new Color(0.7f, 0.7f, 0.7f),
            pressedColor = new Color(0.5f, 0.5f, 0.5f),
            selectedColor = new Color(0.6f, 0.6f, 0.6f),
            disabledColor = new Color(0.45f, 0.45f, 0.45f),
            colorMultiplier = 1
        };

        // 切換按鈕顏色
        if (modeToggleButton != null)
        {
            modeToggleButton.colors = isAutoMode ? autoColors : normalColors;
        }
    }
    #endregion



}
