using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerPlot : MonoBehaviour
{

    private PlotSystem plotSystem;


    public int plotIndex;
    public float delay = 0.0f; // 延遲秒數


    public static bool TriggerClassroomToCorrider; // 教室到走廊
    public static bool TriggerCorriderToClassroom; // 走廊到教室
    public static bool IsTrigger;                 // 觸發後改變
    

    private void Start()
    {

        TriggerClassroomToCorrider = false;
        TriggerCorriderToClassroom = true;
        IsTrigger = false;
        plotSystem = FindObjectOfType<PlotSystem>();
        if (plotSystem == null)
        {
            Debug.LogError("PlotSystem not found in the scene.");
        }

        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            if (plotSystem != null && !IsTrigger && TriggerCorriderToClassroom && TriggerClassroomToCorrider)
            { 
                StartCoroutine(DelayedPlotTrigger());
                IsTrigger = true;
            }
            else
            {
                Debug.Log("Already Interation.");
            }
        }
    }

    private IEnumerator DelayedPlotTrigger()
    {
        yield return new WaitForSeconds(delay);
        plotSystem.WatchPlot(plotIndex);
        gameObject.SetActive(false);
    }
}
