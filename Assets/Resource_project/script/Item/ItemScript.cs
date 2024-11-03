using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    private PlayerMovement playerInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement controller = other.GetComponent<PlayerMovement>();

        if (controller != null)
        {
            playerInRange = controller;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement controller = other.GetComponent<PlayerMovement>();

        if (controller == playerInRange)
        {
            playerInRange = null;
        }
    }

    private void Update()
    {
        // 如果玩家在觸發區域內，並且按下了 "E" 鍵
        if (playerInRange != null && Input.GetKeyDown(KeyCode.E))
        {
            Destroy(gameObject);
        }
    }
}
