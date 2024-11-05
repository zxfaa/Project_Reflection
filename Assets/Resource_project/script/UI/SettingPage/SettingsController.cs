using Flower;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Slider References")]
    public Slider musicVolumeSlider;
    public Slider gameVolumeSlider;
    public Slider textSpeedSlider;
    public Slider dialogAlphaSlider;

    [Header("Mute Button Reference")]
    public Button muteButton; // 靜音按鈕
    private ColorBlock normalColors = new ColorBlock
    {
        normalColor = Color.white,
        highlightedColor = new Color(0.9f, 0.9f, 0.9f),
        pressedColor = new Color(0.75f, 0.75f, 0.75f),
        selectedColor = Color.white,
        disabledColor = new Color(0.6f, 0.6f, 0.6f),
        colorMultiplier = 1
    };

    private ColorBlock muteColors = new ColorBlock
    {
        normalColor = new Color(0.6f, 0.6f, 0.6f),
        highlightedColor = new Color(0.7f, 0.7f, 0.7f),
        pressedColor = new Color(0.5f, 0.5f, 0.5f),
        selectedColor = new Color(0.6f, 0.6f, 0.6f),
        disabledColor = new Color(0.45f, 0.45f, 0.45f),
        colorMultiplier = 1
    };

    private AudioSource backgroundMusic;
    private bool isMuted = false; // 用於記錄靜音狀態

    // 暫存的設定值
    private float tempMusicVolume;
    private float tempGameVolume;
    private float tempTextSpeed;
    private float tempDialogAlpha;

    private void Start()
    {
        InitializeControls();
        SetupListeners();
        FindBackgroundMusic();  // 查找 MusicControl
        LoadCurrentSettings();
        // 確保在啟動時設置正確的顏色狀態
        UpdateMuteButtonColor();
    }

    private void InitializeControls()
    {
        // 設置音量 Slider
        musicVolumeSlider.minValue = 0f;
        musicVolumeSlider.maxValue = 1f;

        //設置遊戲音量Slider
        gameVolumeSlider.minValue = 0f;
        gameVolumeSlider.maxValue = 1f;

        // 設置文字速度 Slider
        textSpeedSlider.minValue = 0f;    // 用於 slider 的顯示範圍
        textSpeedSlider.maxValue = 1f;    // 用於 slider 的顯示範圍

        // 設置對話框透明度 Slider
        dialogAlphaSlider.minValue = 0.6f;
        dialogAlphaSlider.maxValue = 1f;

        // 初始化按鈕顏色
        UpdateMuteButtonColor();
    }

    private void SetupListeners()
    {
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderChanged);
        gameVolumeSlider.onValueChanged.AddListener(OnGameVolumeSliderChanged);
        textSpeedSlider.onValueChanged.AddListener(OnTextSpeedSliderChanged);
        dialogAlphaSlider.onValueChanged.AddListener(OnDialogAlphaSliderChanged);

        // 設置靜音按鈕的點擊事件
        muteButton.onClick.AddListener(OnMuteButtonClick);
    }

    private void FindBackgroundMusic()
    {
        GameObject musicControlObject = GameObject.Find("MusicControl");
        if (musicControlObject != null)
        {
            backgroundMusic = musicControlObject.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogError("未找到名為 'MusicControl' 的物件");
        }
    }

    private void LoadCurrentSettings()
    {
        musicVolumeSlider.value = SettingsManager.MusicVolume;
        gameVolumeSlider.value = SettingsManager.GameVoulume;
        // 將實際速度值轉換為 0-1 的 slider 值
        textSpeedSlider.value = Mathf.InverseLerp(0.02f, 0.005f, SettingsManager.TextSpeed);
        dialogAlphaSlider.value = SettingsManager.DialogAlpha;
        isMuted = SettingsManager.IsMuted;

        tempMusicVolume = musicVolumeSlider.value;
        tempGameVolume = gameVolumeSlider.value;
        tempTextSpeed = textSpeedSlider.value;
        tempDialogAlpha = dialogAlphaSlider.value;

        if (backgroundMusic != null)
        {
            backgroundMusic.volume = tempMusicVolume;
            backgroundMusic.mute = isMuted;
        }
        UpdateMuteButtonColor();
    }

    // Slider 數值改變時的臨時處理方法
    private void OnMusicVolumeSliderChanged(float value)
    {
        tempMusicVolume = value;
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = value; // 即時預覽音量效果
        }
    }
    private void OnGameVolumeSliderChanged(float value)
    {
        tempGameVolume = value;
    }

    private void OnTextSpeedSliderChanged(float value)
    {
        // 將 0-1 的 slider 值轉換為 0.02-0.005 的實際速度值
        // value 為 0 時得到 0.（最慢）
        // value 為 1 時得到 0.005（最快）
        tempTextSpeed = Mathf.Lerp(0.02f, 0.005f, value);
    }

    private void OnDialogAlphaSliderChanged(float value)
    {
        tempDialogAlpha = value;
    }

    // 靜音按鈕點擊事件
    private void OnMuteButtonClick()
    {
        if (backgroundMusic != null)
        {
            // 切換靜音狀態
            isMuted = !isMuted;

            backgroundMusic.mute = isMuted;  // 更新 AudioSource 的靜音狀態

            // 更新靜音狀態到 SettingsManager
            SettingsManager.UpdateIsMuted(isMuted);

            // 更新按鈕顏色狀態
            UpdateMuteButtonColor();
        }
    }

    // 更新靜音按鈕顏色
    private void UpdateMuteButtonColor()
    {
        ColorBlock colors = muteButton.colors;
        // 根據靜音狀態設置對應的 ColorBlock
        muteButton.colors = isMuted ? muteColors : normalColors;
        muteButton.colors = colors;
    }

    public void OnConfirmSettings()
    {
        SettingsManager.UpdateMusicVolume(tempMusicVolume);
        SettingsManager.UpdateGameVolume(tempGameVolume);
        SettingsManager.UpdateTextSpeed(tempTextSpeed);
        SettingsManager.UpdateDialogAlpha(tempDialogAlpha);
        SettingsManager.UpdateIsMuted(isMuted);

        FlowerSystem.textSpeed = tempTextSpeed;
        FlowerSystem.dialogAlpha = tempDialogAlpha;

        Debug.Log("所有設定已更新並保存");
    }

    public void OnCancelSettings()
    {
        LoadCurrentSettings();
        FlowerSystem.textSpeed = SettingsManager.TextSpeed;
        FlowerSystem.dialogAlpha = SettingsManager.DialogAlpha;

        if (backgroundMusic != null)
        {
            backgroundMusic.mute = SettingsManager.IsMuted;  // 恢復靜音狀態
        }

        Debug.Log("設定已還原");
    }

    public void OnResetToDefaults()
    {
        musicVolumeSlider.value = SettingData.Defaults.MUSIC_VOLUME;
        gameVolumeSlider.value = SettingData.Defaults.Game_VOLUME;
        textSpeedSlider.value = SettingData.Defaults.TEXT_SPEED;
        dialogAlphaSlider.value = SettingData.Defaults.DIALOG_ALPHA;
    }
}
