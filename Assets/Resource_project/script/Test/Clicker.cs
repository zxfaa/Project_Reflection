using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clicker : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{ 
    public float requiredHoldTime = 2f;

    [SerializeField] private float pointerDownTimer = 0f;
    [SerializeField] private bool isPointerDown = false;
    [SerializeField] private bool isLongPress = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isLongPress)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                InventorySystem.Instance.CheckLeftClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Reset();
    }

    void Reset()
    {
        pointerDownTimer = 0f;
        isPointerDown = false;
        isLongPress = false;
    }

    private void Update()
    {
        if (isPointerDown)
        {
            pointerDownTimer += Time.deltaTime;

            if (pointerDownTimer >= requiredHoldTime)
            {
                isLongPress = true;
                isPointerDown = false; // ¨¾¤î­«½ÆÄ²µo
                HandleLongPress();
            }
        }
    }

    private void HandleLongPress()
    {
        InventorySystem.Instance.ToggleExamine();
        Reset();
    }
}
