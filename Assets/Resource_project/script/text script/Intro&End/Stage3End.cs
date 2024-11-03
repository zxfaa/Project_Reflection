using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;
public class Stage3End : MonoBehaviour
{
    FlowerSystem fs;
    private int ExtraCount = EncyclopediaUI.ExtraCounter;
    private int NarcissusCount = Narcissus.NarcissusUseCount; 

    private void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");

        fs.SetupUIStage();
        fs.SetupDialog("PlotDialogPrefab");
        if (NarcissusCount > 2)
        {
            //壞結局
            fs.ReadTextFromResource("End/EndAngry");

        }if(NarcissusCount<2 && NarcissusCount>=1)
        {
            //普通結局
            fs.ReadTextFromResource("End/EndArgue");
        }
        if(NarcissusCount == 0 && ExtraCount < 3)
        {
            //好結局
            fs.ReadTextFromResource("End/EndDrepress");
        }
        if(NarcissusCount == 0 && ExtraCount == 3)
        {
            //完美結局
            fs.ReadTextFromResource("End/EndAccept");
        }

    }
}
