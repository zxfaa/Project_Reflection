using Flower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2End: MonoBehaviour
{

    FlowerSystem fs;

    // Update is called once per frame
    private void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        fs.SetupDialog();
        fs.SetupUIStage("default", "DefaultUIStagePrefab", 8);
        fs.ReadTextFromResource("intro&end/stage2end");
    }
}
