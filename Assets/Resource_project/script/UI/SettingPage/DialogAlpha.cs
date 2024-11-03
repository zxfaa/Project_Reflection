using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Flower;

public class DialogAlpha : MonoBehaviour
{
    public Slider alphaSlider;
    void Start()
    {
        // 添加滑動條的事件監聽
        alphaSlider.onValueChanged.AddListener(OnSliderValueChanged);
        // 初始化滑動條的值
        alphaSlider.minValue = 0f;
        alphaSlider.maxValue = 1f;
        alphaSlider.value = FlowerSystem.dialogAlpha;
    }
    public void OnSliderValueChanged(float value)
    {
        FlowerSystem.dialogAlpha = value;
    }
}
