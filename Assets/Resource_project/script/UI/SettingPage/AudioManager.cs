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

    // 檢查音效是否正在播放
    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    // 停止現在播放的音效
    public void StopSound()
    {
        audioSource.Stop();
    }

    public void PlaySound(string soundName)
    {
        if (audioClipDictionary.TryGetValue(soundName, out var clip))
        {
            audioSource.volume = SettingsManager.GameVoulume;
            audioSource.clip = clip;

            // 如果是走路音效，設定為循環播放
            if (soundName == "Walking")
            {
                audioSource.loop = true;
            }
            else
            {
                audioSource.loop = false;
            }

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
