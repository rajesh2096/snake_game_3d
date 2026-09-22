using TMPro;
using UnityEngine;
using SnakeGame3D.Game;

namespace SnakeGame3D.UI
{
    /// <summary>
    /// Displays the live gameplay score HUD.
    /// Listens to ScoreManager.OnScoreChanged events and updates the TextMeshProUGUI element.
    /// Listens to GameOverManager.OnGameOver and GameStartManager events to cleanly hide during Game Over
    /// and reappear when gameplay is ready/started.
    /// </summary>
    public class ScoreHUD : MonoBehaviour
    {
        [Header("Manager References")]
        [Tooltip("Reference to the ScoreManager")]
        [SerializeField] private ScoreManager _scoreManager;

        [Tooltip("Reference to the GameOverManager")]
        [SerializeField] private GameOverManager _gameOverManager;

        [Tooltip("Reference to the GameStartManager")]
        [SerializeField] private GameStartManager _startManager;

        [Header("UI Elements")]
        [Tooltip("Score text component")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Tooltip("Optional root GameObject for HUD visibility toggling")]
        [SerializeField] private GameObject _hudRoot;

        [Header("Configuration")]
        [Tooltip("Format string for the score display")]
        [SerializeField] private string _scoreFormat = "SCORE: {0}";

        public ScoreManager ScoreManager
        {
            get => _scoreManager;
            set
            {
                if (_scoreManager != null)
                {
                    _scoreManager.OnScoreChanged -= HandleScoreChanged;
                }

                _scoreManager = value;

                if (_scoreManager != null && isActiveAndEnabled)
                {
                    _scoreManager.OnScoreChanged += HandleScoreChanged;
                    UpdateScoreText(_scoreManager.CurrentScore);
                }
            }
        }

        public GameOverManager GameOverManager
        {
            get => _gameOverManager;
            set
            {
                if (_gameOverManager != null)
                {
                    _gameOverManager.OnGameOver -= HandleGameOver;
                }

                _gameOverManager = value;

                if (_gameOverManager != null && isActiveAndEnabled)
                {
                    _gameOverManager.OnGameOver += HandleGameOver;
                }
            }
        }

        public GameStartManager StartManager
        {
            get => _startManager;
            set => _startManager = value;
        }

        public TextMeshProUGUI ScoreText
        {
            get => _scoreText;
            set
            {
                _scoreText = value;
                UpdateScoreDisplay();
            }
        }

        public GameObject HUDRoot
        {
            get => _hudRoot;
            set => _hudRoot = value;
        }

        public string ScoreFormat
        {
            get => _scoreFormat;
            set
            {
                _scoreFormat = value;
                UpdateScoreDisplay();
            }
        }

        private void Awake()
        {
            ResolveReferences();
        }

        public void ResolveReferences()
        {
            if (_scoreManager == null)
            {
                _scoreManager = FindAnyObjectByType<ScoreManager>();
            }

            if (_gameOverManager == null)
            {
                _gameOverManager = FindAnyObjectByType<GameOverManager>();
            }

            if (_startManager == null)
            {
                _startManager = FindAnyObjectByType<GameStartManager>();
            }

            if (_scoreText == null)
            {
                _scoreText = GetComponentInChildren<TextMeshProUGUI>(true);
            }

            if (_hudRoot == null)
            {
                _hudRoot = gameObject;
            }
        }

        private void OnEnable()
        {
            ResolveReferences();

            if (_scoreManager != null)
            {
                _scoreManager.OnScoreChanged -= HandleScoreChanged;
                _scoreManager.OnScoreChanged += HandleScoreChanged;
            }

            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
                _gameOverManager.OnGameOver += HandleGameOver;
            }

            // If game is already over when enabled, keep hidden; otherwise show
            if (_gameOverManager != null && _gameOverManager.IsGameOver)
            {
                SetHUDVisibility(false);
            }
            else
            {
                SetHUDVisibility(true);
                UpdateScoreDisplay();
            }
        }

        private void OnDisable()
        {
            if (_scoreManager != null)
            {
                _scoreManager.OnScoreChanged -= HandleScoreChanged;
            }

            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
            }
        }

        /// <summary>
        /// Handles the OnScoreChanged event from ScoreManager.
        /// </summary>
        private void HandleScoreChanged(int score)
        {
            UpdateScoreText(score);
        }

        /// <summary>
        /// Handles Game Over by hiding the live score HUD.
        /// </summary>
        private void HandleGameOver()
        {
            SetHUDVisibility(false);
        }

        /// <summary>
        /// Controls HUD visibility cleanly without destroying components.
        /// </summary>
        public void SetHUDVisibility(bool isVisible)
        {
            if (_hudRoot != null)
            {
                _hudRoot.SetActive(isVisible);
            }
            else if (_scoreText != null)
            {
                _scoreText.gameObject.SetActive(isVisible);
            }
        }

        /// <summary>
        /// Updates the HUD display with the current score from ScoreManager.
        /// </summary>
        public void UpdateScoreDisplay()
        {
            int score = _scoreManager != null ? _scoreManager.CurrentScore : 0;
            UpdateScoreText(score);
        }

        /// <summary>
        /// Formats and updates the TextMeshProUGUI component with the given score value.
        /// </summary>
        public void UpdateScoreText(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = string.Format(_scoreFormat, score);
            }
        }
    }
}
