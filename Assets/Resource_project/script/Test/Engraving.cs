using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Engraving : MonoBehaviour
{
    public Button[] colorButtons;

    private string dragItemName;
    private int countDraw = 0;

    // Start is called before the first frame update
    void Start()
    {
        dragItemName = FindObjectOfType<DragAndDrop>().item.itemName;
    }

    public void Draw(string color)
    {
        if (dragItemName == color)
        {
            //替換為上色圖片
            //使用動畫
            Debug.Log("上色");
            countDraw++;

            DisableButton(color);

            if (countDraw == 3)
            {
                //進入劇情
                Debug.Log("進劇情");
            }
        }
    }

    private void DisableButton(string color)
    {
        Button buttonToDisable = GameObject.Find(color + "Button")?.GetComponent<Button>();
        if (buttonToDisable != null)
        {
            buttonToDisable.interactable = false;
            buttonToDisable.onClick.RemoveAllListeners();
        }
    }
}
