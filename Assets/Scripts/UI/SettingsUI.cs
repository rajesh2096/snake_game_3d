using UnityEngine;
using UnityEngine.UI;
using SnakeGame3D.Game;
using TMPro;

namespace SnakeGame3D.UI
{
    /// <summary>
    /// Manages the Settings UI panel, sound/music/vibration toggles, difficulty selection, and volume sliders.
    /// Supports dynamic discovery and automatic listener binding.
    /// </summary>
    public class SettingsUI : MonoBehaviour
    {
        [Header("Panel Root")]
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
            ResolveReferences();
            BindListeners();
        }

        private void OnEnable()
        {
            ResolveReferences();
            BindListeners();
            RefreshUI();
        }

        private void OnDisable()
        {
            UnbindListeners();
        }

        public void ResolveReferences()
        {
            if (_settingsPanel == null)
            {
                _settingsPanel = gameObject;
            }

            if (_soundToggleButton == null) _soundToggleButton = FindButtonByName("SoundToggle");
            if (_soundToggleText == null && _soundToggleButton != null) _soundToggleText = _soundToggleButton.GetComponentInChildren<TextMeshProUGUI>(true);

            if (_musicToggleButton == null) _musicToggleButton = FindButtonByName("MusicToggle");
            if (_musicToggleText == null && _musicToggleButton != null) _musicToggleText = _musicToggleButton.GetComponentInChildren<TextMeshProUGUI>(true);

            if (_vibrationToggleButton == null) _vibrationToggleButton = FindButtonByName("VibrationToggle");
            if (_vibrationToggleText == null && _vibrationToggleButton != null) _vibrationToggleText = _vibrationToggleButton.GetComponentInChildren<TextMeshProUGUI>(true);

            if (_easyButton == null) _easyButton = FindButtonByName("EasyButton");
            if (_normalButton == null) _normalButton = FindButtonByName("NormalButton");
            if (_hardButton == null) _hardButton = FindButtonByName("HardButton");

            if (_difficultyLabelText == null)
            {
                var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var t in texts)
                {
                    if (t.gameObject.name == "DifficultyLabel")
                    {
                        _difficultyLabelText = t;
                        break;
                    }
                }
            }

            if (_soundVolumeSlider == null)
            {
                var sliders = GetComponentsInChildren<Slider>(true);
                foreach (var s in sliders)
                {
                    if (s.gameObject.name == "SoundVolumeSlider")
                    {
                        _soundVolumeSlider = s;
                        break;
                    }
                }
            }

            if (_musicVolumeSlider == null)
            {
                var sliders = GetComponentsInChildren<Slider>(true);
                foreach (var s in sliders)
                {
                    if (s.gameObject.name == "MusicVolumeSlider")
                    {
                        _musicVolumeSlider = s;
                        break;
                    }
                }
            }

            if (_resetSettingsButton == null) _resetSettingsButton = FindButtonByName("ResetSettingsButton");
            if (_resetBestScoreButton == null) _resetBestScoreButton = FindButtonByName("ResetBestScoreButton");
            if (_backButton == null) _backButton = FindButtonByName("BackButton");
        }

        private Button FindButtonByName(string buttonName)
        {
            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                if (btn.gameObject.name == buttonName)
                {
                    return btn;
                }
            }
            return null;
        }

        private void BindListeners()
        {
            if (_soundToggleButton != null)
            {
                _soundToggleButton.onClick.RemoveListener(OnSoundToggleClicked);
                _soundToggleButton.onClick.AddListener(OnSoundToggleClicked);
            }

            if (_musicToggleButton != null)
            {
                _musicToggleButton.onClick.RemoveListener(OnMusicToggleClicked);
                _musicToggleButton.onClick.AddListener(OnMusicToggleClicked);
            }

            if (_vibrationToggleButton != null)
            {
                _vibrationToggleButton.onClick.RemoveListener(OnVibrationToggleClicked);
                _vibrationToggleButton.onClick.AddListener(OnVibrationToggleClicked);
            }

            if (_easyButton != null)
            {
                _easyButton.onClick.RemoveAllListeners();
                _easyButton.onClick.AddListener(() => OnDifficultyClicked(GameDifficulty.Easy));
            }

            if (_normalButton != null)
            {
                _normalButton.onClick.RemoveAllListeners();
                _normalButton.onClick.AddListener(() => OnDifficultyClicked(GameDifficulty.Normal));
            }

            if (_hardButton != null)
            {
                _hardButton.onClick.RemoveAllListeners();
                _hardButton.onClick.AddListener(() => OnDifficultyClicked(GameDifficulty.Hard));
            }

            if (_soundVolumeSlider != null)
            {
                _soundVolumeSlider.onValueChanged.RemoveListener(OnSoundVolumeChanged);
                _soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
            }

            if (_musicVolumeSlider != null)
            {
                _musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
                _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }

            if (_resetSettingsButton != null)
            {
                _resetSettingsButton.onClick.RemoveListener(OnResetSettingsClicked);
                _resetSettingsButton.onClick.AddListener(OnResetSettingsClicked);
            }

            if (_resetBestScoreButton != null)
            {
                _resetBestScoreButton.onClick.RemoveListener(OnResetBestScoreClicked);
                _resetBestScoreButton.onClick.AddListener(OnResetBestScoreClicked);
            }

            if (_backButton != null)
            {
                _backButton.onClick.RemoveListener(OnBackClicked);
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void UnbindListeners()
        {
            if (_soundToggleButton != null) _soundToggleButton.onClick.RemoveListener(OnSoundToggleClicked);
            if (_musicToggleButton != null) _musicToggleButton.onClick.RemoveListener(OnMusicToggleClicked);
            if (_vibrationToggleButton != null) _vibrationToggleButton.onClick.RemoveListener(OnVibrationToggleClicked);
            if (_soundVolumeSlider != null) _soundVolumeSlider.onValueChanged.RemoveListener(OnSoundVolumeChanged);
            if (_musicVolumeSlider != null) _musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            if (_resetSettingsButton != null) _resetSettingsButton.onClick.RemoveListener(OnResetSettingsClicked);
            if (_resetBestScoreButton != null) _resetBestScoreButton.onClick.RemoveListener(OnResetBestScoreClicked);
            if (_backButton != null) _backButton.onClick.RemoveListener(OnBackClicked);
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
