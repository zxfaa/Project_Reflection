using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Flower;
using System;

public class TeleportManager : MonoBehaviour
{
    public Animator fadeAnimator;
    public Image fadeImage;
    public Transform player;
    public CinemachineVirtualCamera virtualCamera;
    public List<TeleportLocation> teleportLocations;
    public bool isTeleport = false;
    private FlowerSystem fs;

    void Start()
    {
        fadeImage.gameObject.SetActive(false);
        fs = FlowerManager.Instance.GetFlowerSystem("default");


    }

    public void TriggerTeleport(int index, int locationId)
    {
        if (fs.isCompleted)
        {
            isTeleport = true;
            StartCoroutine(TeleportPlayer(index, locationId));
        }
    }

    private IEnumerator TeleportPlayer(int index, int locationId)
    {
        fadeImage.gameObject.SetActive(true);
        fadeAnimator.SetTrigger("FadeInTrigger");

        yield return new WaitForSeconds(1.5f);

        player.position = teleportLocations[index].location[locationId].position;

        virtualCamera.Follow = player;
        virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = teleportLocations[index].confiner;

        fadeAnimator.SetTrigger("FadeOutTrigger");

        yield return new WaitForSeconds(1.5f);

        fadeImage.gameObject.SetActive(false);
        isTeleport = false;
    }

    public int GetCurrentConfinerIndex()
    {
        for (int i = 0; i < teleportLocations.Count; i++)
        {
            if (teleportLocations[i].confiner == virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D)
            {
                return i;
            }
        }
        return -1; // 沒有找到的話返回 -1
    }

    public void SetCameraConfiner(int index)
    {
        if (index >= 0 && index < teleportLocations.Count)
        {
            virtualCamera.Follow = player;
            virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = teleportLocations[index].confiner;
        }
    }
}

[System.Serializable]
public class TeleportLocation
{
    public List<Transform> location; // 傳送點的位置
    public PolygonCollider2D confiner; // 對應的碰撞器
}