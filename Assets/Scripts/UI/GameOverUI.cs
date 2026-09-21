using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SnakeGame3D.Game;

namespace SnakeGame3D.UI
{
    /// <summary>
    /// Manages the Game Over UI panel, score display, best score display, and Restart Button.
    /// Listens to GameOverManager.OnGameOver, queries final score from ScoreManager,
    /// checks/updates BestScoreManager, and invokes GameRestartManager.RestartGame() upon clicking restart.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("Manager References")]
        [Tooltip("Reference to the GameOverManager")]
        [SerializeField] private GameOverManager _gameOverManager;

        [Tooltip("Reference to the ScoreManager")]
        [SerializeField] private ScoreManager _scoreManager;

        [Tooltip("Reference to the BestScoreManager")]
        [SerializeField] private BestScoreManager _bestScoreManager;

        [Tooltip("Reference to the GameRestartManager")]
        [SerializeField] private GameRestartManager _gameRestartManager;

        [Header("UI Elements")]
        [Tooltip("Root panel GameObject for Game Over UI")]
        [SerializeField] private GameObject _gameOverPanel;

        [Tooltip("Title text component")]
        [SerializeField] private TextMeshProUGUI _titleText;

        [Tooltip("Score text component")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Tooltip("Best score text component")]
        [SerializeField] private TextMeshProUGUI _bestScoreText;

        [Tooltip("New Best banner/text indicator")]
        [SerializeField] private GameObject _newBestIndicator;

        [Tooltip("Restart button component")]
        [SerializeField] private Button _restartButton;

        [Header("Configuration")]
        [Tooltip("Score format string")]
        [SerializeField] private string _scoreFormat = "Score: {0}";

        [Tooltip("Best score format string")]
        [SerializeField] private string _bestScoreFormat = "Best: {0}";

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

        public ScoreManager ScoreManager { get => _scoreManager; set => _scoreManager = value; }
        public BestScoreManager BestScoreManager { get => _bestScoreManager; set => _bestScoreManager = value; }
        public GameRestartManager GameRestartManager { get => _gameRestartManager; set => _gameRestartManager = value; }
        public GameObject GameOverPanel { get => _gameOverPanel; set => _gameOverPanel = value; }
        public TextMeshProUGUI TitleText { get => _titleText; set => _titleText = value; }
        public TextMeshProUGUI ScoreText { get => _scoreText; set => _scoreText = value; }
        public TextMeshProUGUI BestScoreText { get => _bestScoreText; set => _bestScoreText = value; }
        public GameObject NewBestIndicator { get => _newBestIndicator; set => _newBestIndicator = value; }

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
            ResolveReferences();
            HidePanel();
        }

        public void ResolveReferences()
        {
            if (_gameOverManager == null) _gameOverManager = FindAnyObjectByType<GameOverManager>();
            if (_scoreManager == null) _scoreManager = FindAnyObjectByType<ScoreManager>();
            if (_bestScoreManager == null) _bestScoreManager = FindAnyObjectByType<BestScoreManager>();
            if (_gameRestartManager == null) _gameRestartManager = FindAnyObjectByType<GameRestartManager>();
        }

        private void OnEnable()
        {
            ResolveReferences();

            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
                _gameOverManager.OnGameOver += HandleGameOver;

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
        /// Handles the OnGameOver event by updating score text, checking best score, and displaying the panel.
        /// </summary>
        private void HandleGameOver()
        {
            ResolveReferences();

            int currentScore = _scoreManager != null ? _scoreManager.CurrentScore : 0;
            bool isNewBest = false;

            if (_bestScoreManager != null)
            {
                isNewBest = _bestScoreManager.TryUpdateBestScore(currentScore);
                UpdateBestScoreText(_bestScoreManager.BestScore);
            }
            else
            {
                int savedBest = PlayerPrefs.GetInt(BestScoreManager.KeyBestScore, 0);
                if (currentScore > savedBest)
                {
                    savedBest = currentScore;
                    PlayerPrefs.SetInt(BestScoreManager.KeyBestScore, savedBest);
                    PlayerPrefs.Save();
                    isNewBest = true;
                }
                UpdateBestScoreText(savedBest);
            }

            UpdateScoreText(currentScore);

            if (_newBestIndicator != null)
            {
                _newBestIndicator.SetActive(isNewBest);
            }

            ShowPanel();
        }

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

        public void UpdateScoreText(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = string.Format(_scoreFormat, score);
            }
        }

        public void UpdateBestScoreText(int bestScore)
        {
            if (_bestScoreText != null)
            {
                _bestScoreText.text = string.Format(_bestScoreFormat, bestScore);
            }
        }

        public void ShowPanel()
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(true);
            }
        }

        public void HidePanel()
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(false);
            }
            if (_newBestIndicator != null)
            {
                _newBestIndicator.SetActive(false);
            }
        }
    }
}
