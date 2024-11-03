using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;

public class TextSpeed : MonoBehaviour
{
    public UnityEngine.UI.Slider speedSlider;  // 引用滑動條
    void Start()
    {

        // 初始化滑動條的值
        speedSlider.minValue = 0.005f;
        speedSlider.maxValue = 0.1f;
        speedSlider.value = FlowerSystem.textSpeed;

        // 添加滑動條的事件監聽
        speedSlider.onValueChanged.AddListener(SetTextSpeed);
    }

    public void SetTextSpeed(float newSpeed)
    {
        FlowerSystem.textSpeed = newSpeed;  // 將滑動條的值設置為文字速度
    }
}
