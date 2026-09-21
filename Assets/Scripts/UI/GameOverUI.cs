using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SnakeGame3D.Game;

namespace SnakeGame3D.UI
{
    /// <summary>
    /// Manages the Game Over UI panel and Restart Button.
    /// Listens to GameOverManager.OnGameOver, queries final score from ScoreManager,
    /// and invokes GameRestartManager.RestartGame() upon clicking the restart button.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("Manager References")]
        [Tooltip("Reference to the GameOverManager")]
        [SerializeField] private GameOverManager _gameOverManager;

        [Tooltip("Reference to the ScoreManager")]
        [SerializeField] private ScoreManager _scoreManager;

        [Tooltip("Reference to the GameRestartManager")]
        [SerializeField] private GameRestartManager _gameRestartManager;

        [Header("UI Elements")]
        [Tooltip("Root panel GameObject for Game Over UI")]
        [SerializeField] private GameObject _gameOverPanel;

        [Tooltip("Title text component")]
        [SerializeField] private TextMeshProUGUI _titleText;

        [Tooltip("Score text component")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Tooltip("Restart button component")]
        [SerializeField] private Button _restartButton;

        [Header("Configuration")]
        [Tooltip("Score format string")]
        [SerializeField] private string _scoreFormat = "Score: {0}";

        public GameOverManager GameOverManager
        {
            get => _gameOverManager;
            set
            {
                if (_gameOverManager != null) _gameOverManager.OnGameOver -= HandleGameOver;
                _gameOverManager = value;
                if (_gameOverManager != null && isActiveAndEnabled) _gameOverManager.OnGameOver += HandleGameOver;
            }
        }

        public ScoreManager ScoreManager
        {
            get => _scoreManager;
            set => _scoreManager = value;
        }

        public GameRestartManager GameRestartManager
        {
            get => _gameRestartManager;
            set => _gameRestartManager = value;
        }

        public GameObject GameOverPanel
        {
            get => _gameOverPanel;
            set => _gameOverPanel = value;
        }

        public TextMeshProUGUI TitleText
        {
            get => _titleText;
            set => _titleText = value;
        }

        public TextMeshProUGUI ScoreText
        {
            get => _scoreText;
            set => _scoreText = value;
        }

        public Button RestartButton
        {
            get => _restartButton;
            set
            {
                if (_restartButton != null) _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
                _restartButton = value;
                if (_restartButton != null && isActiveAndEnabled) _restartButton.onClick.AddListener(OnRestartButtonClicked);
            }
        }

        private void Awake()
        {
            if (_gameOverManager == null)
            {
                _gameOverManager = FindAnyObjectByType<GameOverManager>();
            }

            if (_scoreManager == null)
            {
                _scoreManager = FindAnyObjectByType<ScoreManager>();
            }

            if (_gameRestartManager == null)
            {
                _gameRestartManager = FindAnyObjectByType<GameRestartManager>();
            }

            // Ensure panel is initially hidden
            HidePanel();
        }

        private void OnEnable()
        {
            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
                _gameOverManager.OnGameOver += HandleGameOver;

                // Handle edge case if enabled while already in Game Over
                if (_gameOverManager.IsGameOver)
                {
                    HandleGameOver();
                }
            }

            if (_restartButton != null)
            {
                _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
                _restartButton.onClick.AddListener(OnRestartButtonClicked);
            }
        }

        private void OnDisable()
        {
            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
            }

            if (_restartButton != null)
            {
                _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            }
        }

        /// <summary>
        /// Handles the OnGameOver event by updating score text and displaying the panel.
        /// </summary>
        private void HandleGameOver()
        {
            int currentScore = _scoreManager != null ? _scoreManager.CurrentScore : 0;
            UpdateScoreText(currentScore);
            ShowPanel();
        }

        /// <summary>
        /// Callback when the Restart button is clicked.
        /// </summary>
        public void OnRestartButtonClicked()
        {
            if (_gameRestartManager != null)
            {
                _gameRestartManager.RestartGame();
            }
            else
            {
                Debug.LogWarning("[GameOverUI] GameRestartManager reference is missing!");
            }
        }

        /// <summary>
        /// Updates the score label.
        /// </summary>
        public void UpdateScoreText(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = string.Format(_scoreFormat, score);
            }
        }

        /// <summary>
        /// Activates the Game Over UI panel.
        /// </summary>
        public void ShowPanel()
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Deactivates the Game Over UI panel.
        /// </summary>
        public void HidePanel()
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(false);
            }
        }
    }
}
