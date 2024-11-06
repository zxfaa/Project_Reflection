using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Save : MonoBehaviour
{
    public void SavePlayer(int slotNumber)
    {
        AudioManager.Instance.PlayOneShot("ClickButton");
        SaveSystemSecond.Instance.SavePlayer(slotNumber);
    }

    public void LoadPlayer(int slotNumber)
    {
        AudioManager.Instance.PlayOneShot("ClickButton");
        SaveSystemSecond.Instance.LoadPlayer(slotNumber);
    }



}
