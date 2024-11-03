using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControlManager : MonoBehaviour
{
    public GameObject musicControlPrefab;  //音樂控制的預製物件

    void Start()
    {   //檢查是否存在 MusicControl，如果不存在則重新創建
        MusicControl existingMusicControl = FindObjectOfType<MusicControl>();
        if (existingMusicControl == null)
        {
            Instantiate(musicControlPrefab);
            Debug.Log("不存在，重新創一個");
        }
        else
        {   //音樂控制存在，檢查其 AudioSource 的狀態
            AudioSource audioSource = existingMusicControl.GetComponent<AudioSource>();

            if (audioSource != null)
            {   //檢查是否在播放音樂
                if (!audioSource.isPlaying)
                    existingMusicControl.ResumeMusicFromLastTime();  //如果沒有播放，嘗試播放音樂
            }
        }
    }
}
