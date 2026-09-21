using UnityEngine;
using UnityEngine.UI;
using SnakeGame3D.Game;
using TMPro;

namespace SnakeGame3D.UI
{
    /// <summary>
    /// Manages the Settings UI panel (toggles, sliders, reset, back).
    /// </summary>
    public class SettingsUI : MonoBehaviour
    {
        [Header("Settings Panel Root")]
        [SerializeField] private GameObject _settingsPanel;

        [Header("Toggles & Buttons")]
        [SerializeField] private Button _soundToggleButton;
        [SerializeField] private TextMeshProUGUI _soundToggleText;
        [SerializeField] private Button _musicToggleButton;
        [SerializeField] private TextMeshProUGUI _musicToggleText;
        [SerializeField] private Button _vibrationToggleButton;
        [SerializeField] private TextMeshProUGUI _vibrationToggleText;

        [Header("Sliders")]
        [SerializeField] private Slider _soundVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;

        [Header("Action Buttons")]
        [SerializeField] private Button _resetSettingsButton;
        [SerializeField] private Button _resetBestScoreButton;
        [SerializeField] private Button _backButton;

        public GameObject SettingsPanel { get => _settingsPanel; set => _settingsPanel = value; }
        public Button SoundToggleButton { get => _soundToggleButton; set => _soundToggleButton = value; }
        public Button MusicToggleButton { get => _musicToggleButton; set => _musicToggleButton = value; }
        public Button VibrationToggleButton { get => _vibrationToggleButton; set => _vibrationToggleButton = value; }
        public Slider SoundVolumeSlider { get => _soundVolumeSlider; set => _soundVolumeSlider = value; }
        public Slider MusicVolumeSlider { get => _musicVolumeSlider; set => _musicVolumeSlider = value; }
        public Button ResetSettingsButton { get => _resetSettingsButton; set => _resetSettingsButton = value; }
        public Button ResetBestScoreButton { get => _resetBestScoreButton; set => _resetBestScoreButton = value; }
        public Button BackButton { get => _backButton; set => _backButton = value; }

        private void Awake()
        {
            BindListeners();
        }

        private void OnEnable()
        {
            RefreshUI();
        }

        private void BindListeners()
        {
            if (_soundToggleButton != null) _soundToggleButton.onClick.AddListener(OnSoundToggleClicked);
            if (_musicToggleButton != null) _musicToggleButton.onClick.AddListener(OnMusicToggleClicked);
            if (_vibrationToggleButton != null) _vibrationToggleButton.onClick.AddListener(OnVibrationToggleClicked);

            if (_soundVolumeSlider != null) _soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
            if (_musicVolumeSlider != null) _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            if (_resetSettingsButton != null) _resetSettingsButton.onClick.AddListener(OnResetSettingsClicked);
            if (_resetBestScoreButton != null) _resetBestScoreButton.onClick.AddListener(OnResetBestScoreClicked);
            if (_backButton != null) _backButton.onClick.AddListener(OnBackClicked);
        }

        public void RefreshUI()
        {
            var settings = GameSettingsManager.Instance;
            if (settings == null) return;

            if (_soundToggleText != null)
                _soundToggleText.text = settings.SoundEnabled ? "SOUND: ON" : "SOUND: OFF";

            if (_musicToggleText != null)
                _musicToggleText.text = settings.MusicEnabled ? "MUSIC: ON" : "MUSIC: OFF";

            if (_vibrationToggleText != null)
                _vibrationToggleText.text = settings.VibrationEnabled ? "VIBRATION: ON" : "VIBRATION: OFF";

            if (_soundVolumeSlider != null)
                _soundVolumeSlider.SetValueWithoutNotify(settings.SoundVolume);

            if (_musicVolumeSlider != null)
                _musicVolumeSlider.SetValueWithoutNotify(settings.MusicVolume);
        }

        public void OnSoundToggleClicked()
        {
            if (GameSettingsManager.Instance != null)
            {
                GameSettingsManager.Instance.SoundEnabled = !GameSettingsManager.Instance.SoundEnabled;
                RefreshUI();
            }
        }

        public void OnMusicToggleClicked()
        {
            if (GameSettingsManager.Instance != null)
            {
                GameSettingsManager.Instance.MusicEnabled = !GameSettingsManager.Instance.MusicEnabled;
                RefreshUI();
            }
        }

        public void OnVibrationToggleClicked()
        {
            if (GameSettingsManager.Instance != null)
            {
                GameSettingsManager.Instance.VibrationEnabled = !GameSettingsManager.Instance.VibrationEnabled;
                RefreshUI();
            }
        }

        public void OnSoundVolumeChanged(float val)
        {
            if (GameSettingsManager.Instance != null)
            {
                GameSettingsManager.Instance.SoundVolume = val;
            }
        }

        public void OnMusicVolumeChanged(float val)
        {
            if (GameSettingsManager.Instance != null)
            {
                GameSettingsManager.Instance.MusicVolume = val;
            }
        }

        public void OnResetSettingsClicked()
        {
            if (GameSettingsManager.Instance != null)
            {
                GameSettingsManager.Instance.ResetSettings();
                RefreshUI();
            }
        }

        public void OnResetBestScoreClicked()
        {
            PlayerPrefs.SetInt("SnakeGame_BestScore", 0);
            PlayerPrefs.Save();

            // Refresh MainMenu if present
            MainMenuUI menuUI = FindAnyObjectByType<MainMenuUI>();
            if (menuUI != null)
            {
                menuUI.UpdateBestScoreDisplay();
            }
        }

        public void OnBackClicked()
        {
            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(false);
            }
        }
    }
}
