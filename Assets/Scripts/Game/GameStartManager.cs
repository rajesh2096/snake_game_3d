using System;
using UnityEngine;
using SnakeGame3D.Snake;

namespace SnakeGame3D.Game
{
    /// <summary>
    /// Central manager for the Game Ready / Start state in the 3D Snake Game.
    /// Controls whether active gameplay movement has begun after scene load or restart.
    /// </summary>
    public class GameStartManager : MonoBehaviour
    {
        [Header("System References (Optional manual binding)")]
        [Tooltip("Reference to SnakeController")]
        [SerializeField] private SnakeController _snakeController;

        [Tooltip("Reference to GameOverManager")]
        [SerializeField] private GameOverManager _gameOverManager;

        [Header("State (Read-Only)")]
        [SerializeField] private bool _isGameStarted = false;

        /// <summary>
        /// Event invoked when the game transitions from Ready to Started.
        /// </summary>
        public event Action OnGameStarted;

        /// <summary>
        /// Gets whether the game is currently actively started.
        /// </summary>
        public bool IsGameStarted => _isGameStarted;

        public SnakeController SnakeController
        {
            get => _snakeController;
            set => _snakeController = value;
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

        private void Awake()
        {
            ResolveReferences();
            _isGameStarted = false;
        }

        private void Start()
        {
            // Initial state: READY (movement disabled until START GAME button is clicked)
            if (!_isGameStarted && _snakeController != null)
            {
                _snakeController.StopMovement();
            }
        }

        public void ResolveReferences()
        {
            if (_snakeController == null)
            {
                _snakeController = FindAnyObjectByType<SnakeController>();
            }

            if (_gameOverManager == null)
            {
                _gameOverManager = FindAnyObjectByType<GameOverManager>();
            }
        }

        private void OnEnable()
        {
            ResolveReferences();

            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
                _gameOverManager.OnGameOver += HandleGameOver;
            }
        }

        private void OnDisable()
        {
            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
            }
        }

        /// <summary>
        /// Begins active gameplay by enabling snake movement and broadcasting OnGameStarted.
        /// </summary>
        public void StartGame()
        {
            if (_isGameStarted)
            {
                return;
            }

            if (_gameOverManager != null && _gameOverManager.IsGameOver)
            {
                return;
            }

            _isGameStarted = true;

            if (_snakeController != null)
            {
                _snakeController.StartMovement();
            }

            OnGameStarted?.Invoke();
            Debug.Log("[GameStartManager] Game Started.");
        }

        /// <summary>
        /// Resets the game back to the READY state with snake stopped.
        /// </summary>
        public void ResetStartState()
        {
            _isGameStarted = false;

            if (_snakeController != null)
            {
                _snakeController.StopMovement();
            }

            Debug.Log("[GameStartManager] Start state reset to READY.");
        }

        /// <summary>
        /// Handles GameOver event: marks the game as no longer actively playing and ensures snake stops.
        /// </summary>
        private void HandleGameOver()
        {
            _isGameStarted = false;

            if (_snakeController != null)
            {
                _snakeController.StopMovement();
            }
        }
    }
}
