using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEditor;
using System;
using UnityEngine.UI;
using Cinemachine;


public class CounselingRoomController : MonoBehaviour
{
    int  progress;
    FlowerSystem fs;
    public Item item;

    public static bool IsEnd;
    private bool IsThink;

    private bool isPlayerInRange = false;  // 記錄玩家是否在範圍內
    private bool isInteracting = false;    // 記錄是否正在互動

    public Animator fadeAnimator;
    public Image fadeImage;

    private enum ColorPrint
    {
        None,
        Blue,
        Yellow,
        White
    }
    //默認為Blue
    private ColorPrint currentColor = ColorPrint.None;

    private void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");

        fs.RegisterCommand("pick_item" , (List<string> _params) => {
            item.PickUpItem();
            item.isKeyObject = true;
        });
    }
    void Update()
    {
        // 當玩家在範圍內且按下指定按鍵時觸發交互
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Mouse0) && !isInteracting&&!IsEnd)
        {
            isInteracting = true;  // 設置為正在互動
            IsThink = false;
            fs.SetupUIStage("default", "DefaultUIStagePrefab", 8);
            Play();
        }
    }

    // 當玩家進入觸發範圍時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // 玩家進入範圍
        }
    }

    // 當玩家離開觸發範圍時
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // 玩家離開範圍
        }
    }
    private void Play()
    {
        progress = 0;
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        // 使 fadeImage 顯示
        fadeImage.gameObject.SetActive(true);

        // 觸發 "FadeInTrigger" 動畫
        fadeAnimator.SetTrigger("FadeInTrigger");

        // 等待 1.5 秒
        yield return new WaitForSeconds(1.5f);

        Progress();

        // 觸發 "FadeOutTrigger" 動畫
        fadeAnimator.SetTrigger("FadeOutTrigger");

        // 等待動畫播放結束 
        yield return new WaitForSeconds(1.5f);

        // 停止動畫後，將 fadeImage 隱藏
        fadeImage.gameObject.SetActive(false);


    }
    private void Progress()
    {
        switch (progress)
        {
            case 0:
                fs.SetupDialog("PlotDialogPrefab");
                fs.ReadTextFromResource("CounselingRoom/CounselingRoom");
                progress = 1;
                StartCoroutine(WaitForDialogCompletion());
                break;
            case 1:
                fs.SetupButtonGroup();
                if (!IsThink)
                {
                    fs.SetupButton("先畫畫", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "你畫出了腦中所想的輪廓[w][remove_dialog]" });
                        progress = 2;
                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });
                    fs.SetupButton("再想一下", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "思索片刻，你已有清晰的想法[w][remove_dialog]" });
                        IsThink = true;
                        progress = 1;
                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });
                }
                else
                {
                    fs.SetupButton("畫出心中之物", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "你畫出了腦中所想的輪廓[w][remove_dialog]" });
                        progress = 2;
                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });
                }
                break;
            case 2:
                fs.SetupDialog("PlotDialogPrefab");
                fs.ReadTextFromResource("CounselingRoom/CounselingRoom2");
                progress = 3;
                StartCoroutine(WaitForDialogCompletion());
               
                break;
            case 3:
                fs.SetupButtonGroup();
               
                if( currentColor == ColorPrint.None )
                {
                    fs.SetupButton("藍色", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "你畫上了藍色[w][remove_dialog]" });
                        
                        //下一階段
                        currentColor = ColorPrint.Blue;
                        progress = 4;
                       
                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });
                    fs.SetupButton("黃色", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "：....黃色不應該在這邊[w][remove_dialog]" });
                        
                        //跳轉至白與藍
                        progress = 3;
                        currentColor = ColorPrint.Yellow;
                        
                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });
                    fs.SetupButton("白色", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "：還不到那個時候[w][remove_dialog]" });
                        
                        //跳轉至黃與藍
                        progress = 3;
                        currentColor = ColorPrint.White;
                        
                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });
                }
               if(currentColor == ColorPrint.Yellow)
               {

                    fs.SetupButton("藍色", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "你畫上了藍色[w][remove_dialog]" });

                        //下一階段
                        currentColor = ColorPrint.Blue;
                        progress = 4;

                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });

                    fs.SetupButton("白色", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "：還不到那個時候[w][remove_dialog]" });

                        //跳轉至藍色
                        progress = 3;
                        currentColor = ColorPrint.Blue;

                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });
                }
               if (currentColor == ColorPrint.White)
               {
                    fs.SetupButton("藍色", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "你畫上了藍色[w][remove_dialog]" });

                        //下一階段
                        currentColor = ColorPrint.Blue;
                        progress = 4;

                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });

                    fs.SetupButton("黃色", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "：....黃色不應該在這邊[w][remove_dialog]" });

                        //跳轉至藍色
                        progress = 3;
                        currentColor = ColorPrint.Blue;

                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });

                }
               if(currentColor == ColorPrint.Blue)
                {
                    fs.SetupButton("藍色", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "你畫上了藍色[w][remove_dialog]" });

                        //下一階段
                        currentColor = ColorPrint.Blue;
                        progress = 4;

                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });
                }

                break;
            case 4:
                fs.SetupDialog("PlotDialogPrefab");
                fs.SetTextList(new List<string> { "….白色[w][remove_dialog]" });
                progress = 5;
                StartCoroutine(WaitForDialogCompletion());
                break;
            case 5:
                fs.SetupButtonGroup();
                if(currentColor == ColorPrint.Blue) 
                {
                    fs.SetupButton("黃色", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "：....黃色不應該在這邊[w][remove_dialog]" });

                        //跳轉至白
                        progress = 5;
                        currentColor = ColorPrint.Yellow;

                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });
                    fs.SetupButton("白色", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "你畫上了白色[w][remove_dialog]" });

                        //跳轉至黃
                        progress = 6;
                        currentColor = ColorPrint.Yellow;

                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });
                }
                if(currentColor == ColorPrint.Yellow) 
                {
                    fs.SetupButton("白色", () =>
                    {
                        fs.Resume();
                        fs.SetupDialog();
                        fs.SetTextList(new List<string> { "你畫上了白色[w][remove_dialog]" });

                        //跳轉至白
                        progress = 6;
                        currentColor = ColorPrint.White;

                        fs.RemoveButtonGroup();
                        StartCoroutine(WaitForDialogCompletion());
                    });
                }

                break;
            case 6:
                fs.SetupDialog("PlotDialogPrefab");
                fs.ReadTextFromResource("CounselingRoom/CounselingRoom3");
                IsEnd = true;
                break;

        }


    }

    private IEnumerator WaitForDialogCompletion()
    {
        // 等待对话完成
        while (!fs.isCompleted)
        {
            yield return null; // 每帧检查一次
        }

        // 对话完成后，更新 progress 并继续流程

        Progress();
    }

   
}
