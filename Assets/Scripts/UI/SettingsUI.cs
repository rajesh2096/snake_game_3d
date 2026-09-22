using UnityEngine;
using UnityEngine.UI;
using SnakeGame3D.Game;
using TMPro;

namespace SnakeGame3D.UI
{
    /// <summary>
    /// Manages the Settings UI panel (toggles, sliders, difficulty, reset, back).
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

        [Header("Difficulty Controls")]
        [SerializeField] private Button _easyButton;
        [SerializeField] private Button _normalButton;
        [SerializeField] private Button _hardButton;
        [SerializeField] private TextMeshProUGUI _difficultyLabelText;

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
        public Button EasyButton { get => _easyButton; set => _easyButton = value; }
        public Button NormalButton { get => _normalButton; set => _normalButton = value; }
        public Button HardButton { get => _hardButton; set => _hardButton = value; }
        public TextMeshProUGUI DifficultyLabelText { get => _difficultyLabelText; set => _difficultyLabelText = value; }
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

            if (_easyButton != null) _easyButton.onClick.AddListener(() => OnDifficultyClicked(GameDifficulty.Easy));
            if (_normalButton != null) _normalButton.onClick.AddListener(() => OnDifficultyClicked(GameDifficulty.Normal));
            if (_hardButton != null) _hardButton.onClick.AddListener(() => OnDifficultyClicked(GameDifficulty.Hard));

            if (_soundVolumeSlider != null) _soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
            if (_musicVolumeSlider != null) _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            if (_resetSettingsButton != null) _resetSettingsButton.onClick.AddListener(OnResetSettingsClicked);
            if (_resetBestScoreButton != null) _resetBestScoreButton.onClick.AddListener(OnResetBestScoreClicked);
            if (_backButton != null) _backButton.onClick.AddListener(OnBackClicked);
        }

        public void RefreshUI()
        {
            var settings = GameSettingsManager.Instance;
            if (settings != null)
            {
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

            var diffMgr = GameDifficultyManager.Instance;
            GameDifficulty diff = diffMgr != null ? diffMgr.CurrentDifficulty : (GameDifficulty)PlayerPrefs.GetInt(GameDifficultyManager.KeyDifficulty, (int)GameDifficulty.Normal);

            if (_difficultyLabelText != null)
            {
                _difficultyLabelText.text = $"DIFFICULTY: {diff.ToString().ToUpper()}";
            }

            UpdateDifficultyButtonHighlights(diff);
        }

        private void UpdateDifficultyButtonHighlights(GameDifficulty selectedDiff)
        {
            SetButtonColor(_easyButton, selectedDiff == GameDifficulty.Easy);
            SetButtonColor(_normalButton, selectedDiff == GameDifficulty.Normal);
            SetButtonColor(_hardButton, selectedDiff == GameDifficulty.Hard);
        }

        private void SetButtonColor(Button btn, bool isSelected)
        {
            if (btn == null) return;
            Image img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.color = isSelected ? new Color(0.2f, 0.8f, 0.3f, 1f) : new Color(0.18f, 0.24f, 0.32f, 1f);
            }
        }

        public void OnDifficultyClicked(GameDifficulty diff)
        {
            if (GameDifficultyManager.Instance != null)
            {
                GameDifficultyManager.Instance.SetDifficulty(diff);
            }
            else
            {
                PlayerPrefs.SetInt(GameDifficultyManager.KeyDifficulty, (int)diff);
                PlayerPrefs.Save();
            }
            RefreshUI();
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
            }
            if (GameDifficultyManager.Instance != null)
            {
                GameDifficultyManager.Instance.ResetDifficulty();
            }
            RefreshUI();
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
