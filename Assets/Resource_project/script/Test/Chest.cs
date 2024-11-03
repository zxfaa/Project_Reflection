using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    public GameObject otherStone;
    public Stone[] slots;
    public int openTimes = 0;
    public bool isCorrect = false;

    [SerializeField] private bool isTriggered = false;

    private void Start()
    {
        slots = GetComponentsInChildren<Stone>();

        foreach (var slot in slots)
        {
            slot.SetChestManager(this);
        }
    }

    public void CheckAllSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i >= 6)
            {
                continue;
            }

            if (!slots[i].isItemCorrectlyPlaced)
            {
                Debug.LogWarning("Check");
                return;
            }
        }
        Debug.LogWarning("all correct");
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].enabled = false;
            
            slots[i].GetComponentInParent<Button>().enabled = false;
        }
        TriggerFunction();
    }

    private void TriggerFunction()
    {
        isTriggered = true;
    }

    public void OpenExamine()
    {
        InteractionSystem interactionSystem = FindObjectOfType<InteractionSystem>();
        if (interactionSystem.isExamine && isTriggered)
            openTimes = openTimes + 1;

        if (openTimes == 1)
            HandlePrefabReopened();
    }

    private void HandlePrefabReopened()
    {
        otherStone.SetActive(true);
    }
}
