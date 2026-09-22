using UnityEngine;
using UnityEngine.UI;
using SnakeGame3D.Game;
using TMPro;

namespace SnakeGame3D.UI
{
    /// <summary>
    /// Manages the Main Menu UI.
    /// Handles Play button, Settings button, and Best Score display.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _bestScoreText;
        [SerializeField] private string _bestScoreFormat = "BEST SCORE: {0}";

        [Header("Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;

        [Header("Navigation / Panels")]
        [SerializeField] private GameObject _settingsPanel;

        public TextMeshProUGUI BestScoreText { get => _bestScoreText; set => _bestScoreText = value; }
        public GameObject SettingsPanel { get => _settingsPanel; set => _settingsPanel = value; }
        public Button PlayButton
        {
            get => _playButton;
            set
            {
                if (_playButton != null) _playButton.onClick.RemoveListener(OnPlayClicked);
                _playButton = value;
                if (_playButton != null && isActiveAndEnabled) _playButton.onClick.AddListener(OnPlayClicked);
            }
        }
        public Button SettingsButton
        {
            get => _settingsButton;
            set
            {
                if (_settingsButton != null) _settingsButton.onClick.RemoveListener(OnSettingsClicked);
                _settingsButton = value;
                if (_settingsButton != null && isActiveAndEnabled) _settingsButton.onClick.AddListener(OnSettingsClicked);
            }
        }

        private void Awake()
        {
            ResolveReferences();
            BindListeners();
        }

        private void Start()
        {
            UpdateBestScoreDisplay();
        }

        private void OnEnable()
        {
            ResolveReferences();
            BindListeners();
            UpdateBestScoreDisplay();
        }

        private void OnDisable()
        {
            UnbindListeners();
        }

        public void ResolveReferences()
        {
            if (_playButton == null)
            {
                var buttons = GetComponentsInChildren<Button>(true);
                foreach (var btn in buttons)
                {
                    if (btn.gameObject.name == "PlayButton")
                    {
                        _playButton = btn;
                        break;
                    }
                }
            }

            if (_settingsButton == null)
            {
                var buttons = GetComponentsInChildren<Button>(true);
                foreach (var btn in buttons)
                {
                    if (btn.gameObject.name == "SettingsButton")
                    {
                        _settingsButton = btn;
                        break;
                    }
                }
            }

            if (_bestScoreText == null)
            {
                var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var txt in texts)
                {
                    if (txt.gameObject.name == "BestScoreText")
                    {
                        _bestScoreText = txt;
                        break;
                    }
                }
            }
        }

        private void BindListeners()
        {
            if (_playButton != null)
            {
                _playButton.onClick.RemoveListener(OnPlayClicked);
                _playButton.onClick.AddListener(OnPlayClicked);
            }

            if (_settingsButton != null)
            {
                _settingsButton.onClick.RemoveListener(OnSettingsClicked);
                _settingsButton.onClick.AddListener(OnSettingsClicked);
            }
        }

        private void UnbindListeners()
        {
            if (_playButton != null)
            {
                _playButton.onClick.RemoveListener(OnPlayClicked);
            }

            if (_settingsButton != null)
            {
                _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            }
        }

        public void UpdateBestScoreDisplay()
        {
            int bestScore = PlayerPrefs.GetInt("SnakeGame_BestScore", 0);
            if (_bestScoreText != null)
            {
                _bestScoreText.text = string.Format(_bestScoreFormat, bestScore);
            }
        }

        public void OnPlayClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }

        public void OnSettingsClicked()
        {
            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(true);
            }
        }

        public void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}
