using UnityEngine;
using UnityEngine.UI;
using SnakeGame3D.Game;

namespace SnakeGame3D.UI
{
    /// <summary>
    /// Coordinates the Game Ready / Start UI (StartPanel and START GAME button).
    /// Listens to GameStartManager.OnGameStarted and GameOverManager.OnGameOver.
    /// </summary>
    public class GameStartUI : MonoBehaviour
    {
        [Header("Manager References")]
        [Tooltip("Reference to GameStartManager")]
        [SerializeField] private GameStartManager _startManager;

        [Tooltip("Reference to GameOverManager")]
        [SerializeField] private GameOverManager _gameOverManager;

        [Header("UI Elements")]
        [Tooltip("Root panel GameObject for Start overlay")]
        [SerializeField] private GameObject _startPanel;

        [Tooltip("Start button inside Start panel")]
        [SerializeField] private Button _startButton;

        [Tooltip("Pause button (to show/hide on start)")]
        [SerializeField] private Button _pauseButton;

        public GameStartManager StartManager
        {
            get => _startManager;
            set
            {
                if (_startManager != null)
                {
                    _startManager.OnGameStarted -= HandleGameStarted;
                }
                _startManager = value;
                if (_startManager != null && isActiveAndEnabled)
                {
                    _startManager.OnGameStarted += HandleGameStarted;
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

        public GameObject StartPanel
        {
            get => _startPanel;
            set => _startPanel = value;
        }

        public Button StartButton
        {
            get => _startButton;
            set
            {
                if (_startButton != null) _startButton.onClick.RemoveListener(OnStartButtonClicked);
                _startButton = value;
                if (_startButton != null && isActiveAndEnabled) _startButton.onClick.AddListener(OnStartButtonClicked);
            }
        }

        public Button PauseButton
        {
            get => _pauseButton;
            set => _pauseButton = value;
        }

        private void Awake()
        {
            ResolveReferences();
            UpdateUIState(_startManager != null && _startManager.IsGameStarted);
        }

        public void ResolveReferences()
        {
            if (_startManager == null)
            {
                _startManager = FindAnyObjectByType<GameStartManager>();
            }

            if (_gameOverManager == null)
            {
                _gameOverManager = FindAnyObjectByType<GameOverManager>();
            }

            if (_pauseButton == null)
            {
                PauseUI pauseUI = FindAnyObjectByType<PauseUI>();
                if (pauseUI != null)
                {
                    _pauseButton = pauseUI.PauseButton;
                }
            }
        }

        private void OnEnable()
        {
            ResolveReferences();

            if (_startManager != null)
            {
                _startManager.OnGameStarted -= HandleGameStarted;
                _startManager.OnGameStarted += HandleGameStarted;
            }

            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
                _gameOverManager.OnGameOver += HandleGameOver;
            }

            if (_startButton != null)
            {
                _startButton.onClick.RemoveListener(OnStartButtonClicked);
                _startButton.onClick.AddListener(OnStartButtonClicked);
            }

            UpdateUIState(_startManager != null && _startManager.IsGameStarted);
        }

        private void OnDisable()
        {
            if (_startManager != null)
            {
                _startManager.OnGameStarted -= HandleGameStarted;
            }

            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
            }

            if (_startButton != null)
            {
                _startButton.onClick.RemoveListener(OnStartButtonClicked);
            }
        }

        private void HandleGameStarted()
        {
            UpdateUIState(true);
        }

        private void HandleGameOver()
        {
            if (_startPanel != null)
            {
                _startPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Updates UI visibility based on whether the game is started.
        /// </summary>
        public void UpdateUIState(bool isStarted)
        {
            bool isGameOver = _gameOverManager != null && _gameOverManager.IsGameOver;

            if (isGameOver)
            {
                if (_startPanel != null) _startPanel.SetActive(false);
                if (_pauseButton != null) _pauseButton.gameObject.SetActive(false);
                return;
            }

            if (isStarted)
            {
                if (_startPanel != null) _startPanel.SetActive(false);
                if (_pauseButton != null) _pauseButton.gameObject.SetActive(true);
            }
            else
            {
                if (_startPanel != null) _startPanel.SetActive(true);
                if (_pauseButton != null) _pauseButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Handles click on START GAME button.
        /// </summary>
        public void OnStartButtonClicked()
        {
            if (_startManager != null)
            {
                _startManager.StartGame();
            }
        }
    }
}
