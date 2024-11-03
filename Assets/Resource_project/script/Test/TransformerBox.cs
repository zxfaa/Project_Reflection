using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformerBox : MonoBehaviour
{
    public bool isKeyUsed;
    Item itemVariable;

    public void OpenBox()
    {
        itemVariable = GetComponent<Item>();
        isKeyUsed = true;
        itemVariable.dialogueType = Item.DialogueType.NONE;
    }

    public void TurnOnLight()
    {
        if (isKeyUsed)
        {
            itemVariable.TriggerDialogue(dialogueIndex: 21);
            FindObjectOfType<LeftToRightFadeController>().fade = true;
            gameObject.SetActive(false);
        }   
    }
}
