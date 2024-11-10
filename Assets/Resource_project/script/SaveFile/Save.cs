using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Save : MonoBehaviour
{
    private int nowPlayerNumber = SaveSystemSecond.currentPlayerNumber;
    public void SavePlayer()
    {
        AudioManager.Instance.PlayOneShot("ClickButton");
        SaveSystemSecond.Instance.SavePlayer(nowPlayerNumber);
    }

    //在首頁指定存檔編號時使用
    public void LoadPlayerNumber(int slotNumber)
    {
        AudioManager.Instance.PlayOneShot("ClickButton");
        SaveSystemSecond.Instance.LoadPlayer(slotNumber);
    }
    public void LoadPlayer()
    {
        AudioManager.Instance.PlayOneShot("ClickButton");
        SaveSystemSecond.Instance.LoadPlayer(nowPlayerNumber);

    }
    public void StarGame(int SceneIndex)
    {
        AudioManager.Instance.PlayOneShot("ClickButton");
        SaveSystemSecond.Instance.StartGame(SceneIndex);

    }



}
