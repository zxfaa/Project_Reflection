using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private RectTransform rectTransform; // 拖曳物件的RectTransform
    private Image dragImage;
    public RectTransform canvas; // 所屬的Canvas，用於計算鼠標位置

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); // 獲取當前物件的RectTransform
        dragImage = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 將鼠標位置轉換為Canvas內的本地位置
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            rectTransform.anchoredPosition = localPointerPosition; // 更新圖片的位置
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragImage.raycastTarget = true;
    }
}
