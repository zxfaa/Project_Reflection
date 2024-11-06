using Flower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public bool isKeyUsed = false;
    public bool isDoorUnlocked = false;
    public GameObject shareStareObject;

    public void Door()
    {
        Item itemVariable = GetComponent<Item>();
        UseKey();
        if (isKeyUsed)
        {
            itemVariable.TriggerDialogue(13);
            UnlockDoor();
            itemVariable.dialogueType = Item.DialogueType.NONE;
        }
    }

    public void UseKey()
    {
        isKeyUsed = true;
        AudioManager.Instance.PlayOneShot("Key on door s");
        Debug.Log("鑰匙已使用，傳送功能啟用");
    }

    public void UnlockDoor()
    {
        if (isKeyUsed)
        {
            isDoorUnlocked = true;
            if (shareStareObject != null)
                ShareState();
            Debug.Log("門已解鎖，可以進行傳送");
        }
        else
        {
            Debug.Log("請先使用鑰匙解鎖門");
        }
    }

    public void ShareState()
    {
        Item itemVariable = shareStareObject.GetComponent<Item>();
        shareStareObject.GetComponent<OpenDoor>().isKeyUsed = true;
        shareStareObject.GetComponent<OpenDoor>().isDoorUnlocked = true;
        itemVariable.dialogueType = Item.DialogueType.NONE;
        itemVariable.itemType = Item.ItemType.NONE;
    }
}
