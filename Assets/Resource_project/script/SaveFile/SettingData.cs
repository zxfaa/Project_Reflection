using System;

[Serializable]
public class SettingData
{
    public float MusicVolume = 0.5f;
    public float GameVolume;
    public bool IsMuted = false;  // 新增靜音狀態
    public float TextSpeed = 0.02f;
    public float DialogAlpha = 1.0f;

    // 默認值定義
    public static class Defaults
    {
        public const float MUSIC_VOLUME = 0.5f;
        public const float Game_VOLUME = 0.3f;
        public const bool IS_MUTED = false;
        public const float TEXT_SPEED = 0.02f;
        public const float DIALOG_ALPHA = 1.0f;
    }
}