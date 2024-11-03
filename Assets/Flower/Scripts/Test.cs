using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;

public class Test : MonoBehaviour
{
    FlowerSystem fs;
    // Start is called before the first frame update
    void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        fs.SetupUIStage();
        fs.SetupDialog("PlotDialogPrefab");
        fs.ReadTextFromResource("Test");
    }

   
}
