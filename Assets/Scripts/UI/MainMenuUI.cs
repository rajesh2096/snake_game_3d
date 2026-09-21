using UnityEngine;
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

        [Header("Navigation / Panels")]
        [SerializeField] private GameObject _settingsPanel;

        public TextMeshProUGUI BestScoreText { get => _bestScoreText; set => _bestScoreText = value; }
        public GameObject SettingsPanel { get => _settingsPanel; set => _settingsPanel = value; }

        private void Start()
        {
            UpdateBestScoreDisplay();
        }

        private void OnEnable()
        {
            UpdateBestScoreDisplay();
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
