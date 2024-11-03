using System.Collections.Generic;
using UnityEngine;

public class ItemWheelController : MonoBehaviour
{
    public List<Transform> items; // 所有物品的Transform列表
    public float rotationSpeed = 100f; // 旋轉速度
    private int currentIndex = 0; // 當前選中的物品索引

    private void Start()
    {
        if (items == null || items.Count == 0)
        {
            Debug.LogError("物品輪盤中未分配任何物品。");
            return;
        }

        UpdateItemPositions();
    }

    private void Update()
    {
        // 檢測滑鼠滾輪輸入
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            if (scroll > 0)
            {
                SelectNextItem();
            }
            else if (scroll < 0)
            {
                SelectPreviousItem();
            }
        }
    }

    private void SelectNextItem()
    {
        currentIndex = (currentIndex + 1) % items.Count;
        UpdateItemPositions();
    }

    private void SelectPreviousItem()
    {
        currentIndex = (currentIndex - 1 + items.Count) % items.Count;
        UpdateItemPositions();
    }

    private void UpdateItemPositions()
    {
        float angleStep = 360f / items.Count;
        float currentAngle = -currentIndex * angleStep;

        for (int i = 0; i < items.Count; i++)
        {
            float angle = currentAngle + i * angleStep;
            Vector3 position = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            items[i].localPosition = position;
            items[i].localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}