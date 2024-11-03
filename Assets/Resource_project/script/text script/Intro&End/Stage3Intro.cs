using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;

public class Stage3IntroD : MonoBehaviour
{
    FlowerSystem fs;

    // Update is called once per frame
    private void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        fs.SetupDialog();
        fs.SetupUIStage("default", "DefaultUIStagePrefab", 8);
        fs.ReadTextFromResource("intro&end/stage3intro");
    }
}
