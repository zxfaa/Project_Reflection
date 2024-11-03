using UnityEngine;
public static class MusicState
{
    public static float Volume = SettingsManager.MusicVolume;  //保存音量狀態
    public static bool IsMuted = SettingsManager.IsMuted; //保存靜音狀態;  
}
public class MusicControl : MonoBehaviour
{
    private static MusicControl instance;       //靜態變數用來確保單例模式
    private AudioSource audioSource;            //音樂的音源組件
    private static float lastPlaybackTime = 0f; //用來儲存音樂的播放進度
    void Awake()    //實例配套
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    //初始化 AudioSource 並從上次進度繼續播放音樂
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //初始化音量和靜音狀態
        audioSource.volume = MusicState.Volume;
        audioSource.mute = MusicState.IsMuted;
        //從上次進度繼續播放音樂
        ResumeMusicFromLastTime();
    }
    // 每幀更新當前的播放進度，以便在場景切換後能夠接續播放
    void Update()
    {
        if (audioSource != null && audioSource.isPlaying)
            lastPlaybackTime = audioSource.time;    //記錄當前播放進度
    }
    public void ResumeMusicFromLastTime()
    {
        if (audioSource != null)
        {
            if (lastPlaybackTime > 0)
            {
                // 如果有保存的進度，從該進度繼續播放
                audioSource.time = lastPlaybackTime;
                audioSource.Play();
            }
            else                
                audioSource.Play();     // 如果沒有保存的進度，從頭開始播放
        }
    }

}