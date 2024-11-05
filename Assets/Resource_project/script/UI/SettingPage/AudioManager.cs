using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("不同類型的音效")]
    public AudioSource loopingAudioSource;    // 專門用於循環播放的音效
    public List<AudioClip> audioClips;        // 在 Inspector 中添加所有音效文件（AudioClip）

    private Dictionary<string, AudioClip> audioClipDictionary;
    private List<AudioSource> oneShotAudioSources; // 管理所有單次播放的 AudioSource

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioClips();
            oneShotAudioSources = new List<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 初始化音效字典
    private void InitializeAudioClips()
    {
        audioClipDictionary = new Dictionary<string, AudioClip>();

        foreach (var clip in audioClips)
        {
            if (clip != null)
            {
                audioClipDictionary[clip.name] = clip;
            }
        }
    }

    // 判斷是否正在播放循環音效
    public bool IsLoopingSoundPlaying()
    {
        return loopingAudioSource.isPlaying;
    }

    // 停止播放循環音效
    public void StopLoopingSound()
    {
        loopingAudioSource.Stop();
    }

    // 播放循環音效，例如腳步聲
    public void PlayLoopingSound(string soundName)
    {
        if (audioClipDictionary.TryGetValue(soundName, out var clip))
        {
            loopingAudioSource.Stop();
            loopingAudioSource.loop = true;
            loopingAudioSource.clip = clip;
            loopingAudioSource.volume = SettingsManager.GameVoulume;
            loopingAudioSource.Play();
        }
    }

    // 播放單次音效，例如開門聲
    public void PlayOneShot(string soundName)
    {
        if (audioClipDictionary.TryGetValue(soundName, out var clip))
        {
            AudioSource oneShotSource = gameObject.AddComponent<AudioSource>();
            oneShotSource.volume = SettingsManager.GameVoulume;
            oneShotSource.PlayOneShot(clip);

            oneShotAudioSources.Add(oneShotSource);
            StartCoroutine(DestroyOneShotSourceAfterPlay(oneShotSource, clip.length));
        }
    }

    // 播放完成後銷毀單次音效的 AudioSource
    private System.Collections.IEnumerator DestroyOneShotSourceAfterPlay(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        oneShotAudioSources.Remove(source);
        Destroy(source);
    }

    // 常用音效的快捷方法
    public void PlayOpenDoor() => PlayOneShot("OpenDoor");
    public void PlayWalking() => PlayLoopingSound("Walking");
}
