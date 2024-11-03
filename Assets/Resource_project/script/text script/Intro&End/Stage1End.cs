using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class Stage1End : MonoBehaviour
{
    // Start is called before the first frame update

    FlowerSystem fs;

    // Update is called once per frame
    private void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        fs.SetupDialog();
        fs.SetupUIStage("default", "DefaultUIStagePrefab", 8);
        fs.ReadTextFromResource("intro&end/stage1end");
    }
}
