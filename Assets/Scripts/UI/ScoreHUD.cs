using TMPro;
using UnityEngine;
using SnakeGame3D.Game;

namespace SnakeGame3D.UI
{
    /// <summary>
    /// Displays the live gameplay score HUD.
    /// Listens to ScoreManager.OnScoreChanged events and updates the TextMeshProUGUI element.
    /// </summary>
    public class ScoreHUD : MonoBehaviour
    {
        [Header("Manager Reference")]
        [Tooltip("Reference to the ScoreManager")]
        [SerializeField] private ScoreManager _scoreManager;

        [Header("UI Elements")]
        [Tooltip("Score text component")]
        [SerializeField] private TextMeshProUGUI _scoreText;

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

        public TextMeshProUGUI ScoreText
        {
            get => _scoreText;
            set
            {
                _scoreText = value;
                UpdateScoreDisplay();
            }
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
            if (_scoreManager == null)
            {
                _scoreManager = FindAnyObjectByType<ScoreManager>();
            }

            if (_scoreText == null)
            {
                _scoreText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        private void OnEnable()
        {
            if (_scoreManager != null)
            {
                _scoreManager.OnScoreChanged -= HandleScoreChanged;
                _scoreManager.OnScoreChanged += HandleScoreChanged;
            }

            UpdateScoreDisplay();
        }

        private void OnDisable()
        {
            if (_scoreManager != null)
            {
                _scoreManager.OnScoreChanged -= HandleScoreChanged;
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
