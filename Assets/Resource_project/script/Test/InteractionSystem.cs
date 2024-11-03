using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Flower;

public class InteractionSystem : MonoBehaviour
{
    [Header("Detection Field")]
    public Transform detectionPoint;
    public const float detectionRadius = 0.2f;
    public LayerMask detectionLayer;
    public GameObject detectionObject;

    [Header("Examine Field")]
    public GameObject examineWindow;
    public GameObject menuBar;
    public GameObject narcissus;
    public bool isExamine;

    FlowerSystem fs;

    private void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
    }
    void Update()
    {
        if (isExamine)
            return;
        if (InventorySystem.Instance.isOpen)
            return;
        if (!fs.isCompleted)
            return;
        if (FindObjectOfType<Player>().isOpeningUI)
            return;

        if (DetectObject() && IsMouseOverObject())
        {
            if (InteractionInput())
            {
                detectionObject.GetComponent<Item>().Interact();
            }
        }
    }

    bool InteractionInput()
    {
        return Input.GetMouseButtonDown(0);
    }

    bool DetectObject()
    {
        Collider2D obj = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);

        if (obj == null)
        {
            detectionObject = null;
            return false;
        }
        else
        {
            detectionObject = obj.gameObject;
            return true;
        }
    }

    bool IsMouseOverObject()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, detectionLayer);
        return hit.collider != null && hit.collider.gameObject == detectionObject;
    }

    public void ExamineObject()
    {
        isExamine = !isExamine;
        Item examine = detectionObject.GetComponent<Item>();

        if (isExamine)
        {
            GameObject instance = examine.GetOrCreateExaminePrefab();
            instance.transform.SetParent(examine.prefabContainer);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            instance.SetActive(true);
        }
        else
        {
            if (examine.instantiatedExaminePrefab != null)
            {
                examine.instantiatedExaminePrefab.SetActive(false);
                fs.StopAndReset();
            }
        }

        menuBar.SetActive(!isExamine);
        examineWindow.SetActive(isExamine);
        narcissus.SetActive(!isExamine);
    }
}
