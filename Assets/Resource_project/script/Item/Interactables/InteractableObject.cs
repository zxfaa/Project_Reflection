using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public virtual void OnItemUsed(DraggableItem item)
    {
        Debug.Log("使用了道具：" + item.name + " 在物件：" + gameObject.name);
    }
}
