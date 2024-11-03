using UnityEngine;
using UnityEngine.EventSystems;

public class Maze : MonoBehaviour, IPointerEnterHandler
{
    private Item mazeKey;

    void Start()
    {
        mazeKey = GetComponent<Item>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 當拖曳物件進入目標區域時觸發
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            Debug.Log("Dropped object");
            // 在這裡處理具體的事件邏輯
            mazeKey.Interact();
        }
        else
        {
            Debug.Log("Dropped object is null");
        }
    }
}
