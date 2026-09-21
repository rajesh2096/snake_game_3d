using UnityEngine;
using UnityEngine.UI;
using SnakeGame3D.Game;

namespace SnakeGame3D.UI
{
    /// <summary>
    /// Coordinates Pause UI visibility and button click handlers.
    /// Listens to GamePauseManager.OnPauseStateChanged and GameOverManager.OnGameOver events.
    /// </summary>
    public class PauseUI : MonoBehaviour
    {
        [Header("Manager References")]
        [Tooltip("Reference to GamePauseManager")]
        [SerializeField] private GamePauseManager _pauseManager;

        [Tooltip("Reference to GameOverManager")]
        [SerializeField] private GameOverManager _gameOverManager;

        [Tooltip("Reference to GameStartManager")]
        [SerializeField] private GameStartManager _startManager;

        [Header("UI Elements")]
        [Tooltip("Pause button (usually top-right HUD)")]
        [SerializeField] private Button _pauseButton;

        [Tooltip("Root panel GameObject for Pause overlay")]
        [SerializeField] private GameObject _pausePanel;

        [Tooltip("Resume button inside Pause panel")]
        [SerializeField] private Button _resumeButton;

        public GamePauseManager PauseManager
        {
            get => _pauseManager;
            set
            {
                if (_pauseManager != null)
                {
                    _pauseManager.OnPauseStateChanged -= HandlePauseStateChanged;
                }
                _pauseManager = value;
                if (_pauseManager != null && isActiveAndEnabled)
                {
                    _pauseManager.OnPauseStateChanged += HandlePauseStateChanged;
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

        public Button PauseButton
        {
            get => _pauseButton;
            set
            {
                if (_pauseButton != null) _pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
                _pauseButton = value;
                if (_pauseButton != null && isActiveAndEnabled) _pauseButton.onClick.AddListener(OnPauseButtonClicked);
            }
        }

        public GameObject PausePanel
        {
            get => _pausePanel;
            set => _pausePanel = value;
        }

        public Button ResumeButton
        {
            get => _resumeButton;
            set
            {
                if (_resumeButton != null) _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
                _resumeButton = value;
                if (_resumeButton != null && isActiveAndEnabled) _resumeButton.onClick.AddListener(OnResumeButtonClicked);
            }
        }

        private void Awake()
        {
            ResolveReferences();
            UpdateUIState(_pauseManager != null && _pauseManager.IsPaused);
        }

        public void ResolveReferences()
        {
            if (_pauseManager == null)
            {
                _pauseManager = FindAnyObjectByType<GamePauseManager>();
            }

            if (_gameOverManager == null)
            {
                _gameOverManager = FindAnyObjectByType<GameOverManager>();
            }

            if (_startManager == null)
            {
                _startManager = FindAnyObjectByType<GameStartManager>();
            }
        }

        private void OnEnable()
        {
            ResolveReferences();

            if (_pauseManager != null)
            {
                _pauseManager.OnPauseStateChanged -= HandlePauseStateChanged;
                _pauseManager.OnPauseStateChanged += HandlePauseStateChanged;
            }

            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
                _gameOverManager.OnGameOver += HandleGameOver;
            }

            if (_pauseButton != null)
            {
                _pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
                _pauseButton.onClick.AddListener(OnPauseButtonClicked);
            }

            if (_resumeButton != null)
            {
                _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
                _resumeButton.onClick.AddListener(OnResumeButtonClicked);
            }

            UpdateUIState(_pauseManager != null && _pauseManager.IsPaused);
        }

        private void OnDisable()
        {
            if (_pauseManager != null)
            {
                _pauseManager.OnPauseStateChanged -= HandlePauseStateChanged;
            }

            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
            }

            if (_pauseButton != null)
            {
                _pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
            }

            if (_resumeButton != null)
            {
                _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            }
        }

        /// <summary>
        /// Handles pause state transitions from GamePauseManager.
        /// </summary>
        private void HandlePauseStateChanged(bool isPaused)
        {
            UpdateUIState(isPaused);
        }

        /// <summary>
        /// Handles Game Over: hides pause button and pause panel.
        /// </summary>
        private void HandleGameOver()
        {
            if (_pausePanel != null)
            {
                _pausePanel.SetActive(false);
            }

            if (_pauseButton != null)
            {
                _pauseButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Updates visibility of PauseButton and PausePanel based on current pause, game-over, and start state.
        /// </summary>
        public void UpdateUIState(bool isPaused)
        {
            bool isGameOver = _gameOverManager != null && _gameOverManager.IsGameOver;
            bool isStarted = _startManager == null || _startManager.IsGameStarted;

            if (isGameOver || !isStarted)
            {
                if (_pauseButton != null) _pauseButton.gameObject.SetActive(false);
                if (_pausePanel != null) _pausePanel.SetActive(false);
                return;
            }

            if (isPaused)
            {
                if (_pauseButton != null) _pauseButton.gameObject.SetActive(false);
                if (_pausePanel != null) _pausePanel.SetActive(true);
            }
            else
            {
                if (_pauseButton != null) _pauseButton.gameObject.SetActive(true);
                if (_pausePanel != null) _pausePanel.SetActive(false);
            }
        }

        /// <summary>
        /// Callback for Pause button click.
        /// </summary>
        public void OnPauseButtonClicked()
        {
            if (_pauseManager != null)
            {
                _pauseManager.PauseGame();
            }
        }

        /// <summary>
        /// Callback for Resume button click.
        /// </summary>
        public void OnResumeButtonClicked()
        {
            if (_pauseManager != null)
            {
                _pauseManager.ResumeGame();
            }
        }
    }
}
