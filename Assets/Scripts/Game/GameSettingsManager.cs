using UnityEngine;
using SnakeGame3D.Audio;

namespace SnakeGame3D.Game
{
    /// <summary>
    /// Central manager for game settings (Sound, Music, Volume, Vibration).
    /// Persists settings to PlayerPrefs and applies them directly to GameAudioManager and BackgroundMusicManager.
    /// </summary>
    public class GameSettingsManager : MonoBehaviour
    {
        public const string KeySoundEnabled = "SnakeGame_SoundEnabled";
        public const string KeyMusicEnabled = "SnakeGame_MusicEnabled";
        public const string KeySoundVolume = "SnakeGame_SoundVolume";
        public const string KeyMusicVolume = "SnakeGame_MusicVolume";
        public const string KeyVibrationEnabled = "SnakeGame_VibrationEnabled";

        private static GameSettingsManager _instance;
        public static GameSettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<GameSettingsManager>();
                }
                return _instance;
            }
        }

        [Header("Settings Cache")]
        [SerializeField] private bool _soundEnabled = true;
        [SerializeField] private bool _musicEnabled = true;
        [SerializeField] private float _soundVolume = 1f;
        [SerializeField] private float _musicVolume = 0.6f;
        [SerializeField] private bool _vibrationEnabled = true;

        [Header("Manager References (Optional)")]
        [SerializeField] private GameAudioManager _audioManager;
        [SerializeField] private BackgroundMusicManager _musicManager;

        public bool SoundEnabled
        {
            get => _soundEnabled;
            set
            {
                _soundEnabled = value;
                SaveSettings();
                ApplySettings();
            }
        }

        public bool MusicEnabled
        {
            get => _musicEnabled;
            set
            {
                _musicEnabled = value;
                SaveSettings();
                ApplySettings();
            }
        }

        public float SoundVolume
        {
            get => _soundVolume;
            set
            {
                _soundVolume = Mathf.Clamp01(value);
                SaveSettings();
                ApplySettings();
            }
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Mathf.Clamp01(value);
                SaveSettings();
                ApplySettings();
            }
        }

        public bool VibrationEnabled
        {
            get => _vibrationEnabled;
            set
            {
                _vibrationEnabled = value;
                SaveSettings();
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            LoadSettings();
        }

        private void Start()
        {
            ApplySettings();
        }

        public void LoadSettings()
        {
            _soundEnabled = PlayerPrefs.GetInt(KeySoundEnabled, 1) == 1;
            _musicEnabled = PlayerPrefs.GetInt(KeyMusicEnabled, 1) == 1;
            _soundVolume = PlayerPrefs.GetFloat(KeySoundVolume, 1f);
            _musicVolume = PlayerPrefs.GetFloat(KeyMusicVolume, 0.6f);
            _vibrationEnabled = PlayerPrefs.GetInt(KeyVibrationEnabled, 1) == 1;
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetInt(KeySoundEnabled, _soundEnabled ? 1 : 0);
            PlayerPrefs.SetInt(KeyMusicEnabled, _musicEnabled ? 1 : 0);
            PlayerPrefs.SetFloat(KeySoundVolume, _soundVolume);
            PlayerPrefs.SetFloat(KeyMusicVolume, _musicVolume);
            PlayerPrefs.SetInt(KeyVibrationEnabled, _vibrationEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ResetSettings()
        {
            _soundEnabled = true;
            _musicEnabled = true;
            _soundVolume = 1f;
            _musicVolume = 0.6f;
            _vibrationEnabled = true;
            SaveSettings();
            ApplySettings();
        }

        public void ApplySettings()
        {
            if (_audioManager == null)
            {
                _audioManager = FindAnyObjectByType<GameAudioManager>();
            }

            if (_musicManager == null)
            {
                _musicManager = FindAnyObjectByType<BackgroundMusicManager>();
            }

            if (_audioManager != null)
            {
                _audioManager.SetSoundEnabled(_soundEnabled);
                _audioManager.SetSoundVolume(_soundVolume);
            }

            if (_musicManager != null)
            {
                _musicManager.SetMusicEnabled(_musicEnabled);
                _musicManager.SetMusicVolume(_musicVolume);
            }
        }
    }
}
