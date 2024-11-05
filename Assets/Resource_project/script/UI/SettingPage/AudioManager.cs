using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("不同類型的音效")]
    public AudioSource audioSource;   //  AudioSource
    public List<AudioClip> audioClips; // 在 Inspector 中添加所有音效文件（AudioClip）

    private Dictionary<string, AudioClip> audioClipDictionary; // 儲存音效名稱和對應的 AudioClip

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioClips();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioClips()
    {
        audioClipDictionary = new Dictionary<string, AudioClip>();

        foreach (var clip in audioClips)
        {
            if (clip != null)
            {
                audioClipDictionary[clip.name] = clip; // 使用音效名稱作為鍵
            }
        }
    }

    public void PlaySound(string soundName)
    {
        if (audioClipDictionary.TryGetValue(soundName, out var clip))
        {
            audioSource.volume = SettingsManager.GameVoulume;  // 設定音量
            audioSource.clip = clip;                          // 設定音效
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"音效 '{soundName}' 不存在！");
        }
    }

    public void PlayOpenDoor() => PlaySound("OpenDoor");
    public void PlayWalking() => PlaySound("Walking");
}
