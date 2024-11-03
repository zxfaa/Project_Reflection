using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Save : MonoBehaviour
{
    public void SavePlayer(int slotNumber)
    {
        SaveSystemSecond.Instance.SavePlayer(slotNumber);
    }

    public void LoadPlayer(int slotNumber)
    {
        SaveSystemSecond.Instance.LoadPlayer(slotNumber);
    }



}
