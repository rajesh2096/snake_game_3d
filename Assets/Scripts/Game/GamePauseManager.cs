using System;
using UnityEngine;

namespace SnakeGame3D.Game
{
    /// <summary>
    /// Central manager for Game Pause and Resume states in the 3D Snake Game.
    /// Manages Time.timeScale, broadcasts pause state change events, and coordinates with GameOverManager.
    /// </summary>
    public class GamePauseManager : MonoBehaviour
    {
        [Header("System References (Optional manual binding)")]
        [Tooltip("Reference to GameOverManager")]
        [SerializeField] private GameOverManager _gameOverManager;

        [Header("State (Read-Only)")]
        [SerializeField] private bool _isPaused = false;

        /// <summary>
        /// Event invoked when pause state changes, passing true if paused, false if resumed.
        /// </summary>
        public event Action<bool> OnPauseStateChanged;

        /// <summary>
        /// Gets whether the game is currently paused.
        /// </summary>
        public bool IsPaused => _isPaused;

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
            _isPaused = false;
            Time.timeScale = 1f;

            if (_gameOverManager == null)
            {
                _gameOverManager = FindAnyObjectByType<GameOverManager>();
            }
        }

        private void OnEnable()
        {
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

            // Restore normal time scale if disabled
            Time.timeScale = 1f;
        }

        /// <summary>
        /// Pauses the game if not already paused and not in Game Over state.
        /// Sets Time.timeScale to 0 and invokes OnPauseStateChanged(true).
        /// </summary>
        public void PauseGame()
        {
            // Do not allow pausing if game over or already paused
            if (_isPaused)
            {
                return;
            }

            if (_gameOverManager != null && _gameOverManager.IsGameOver)
            {
                return;
            }

            _isPaused = true;
            Time.timeScale = 0f;
            OnPauseStateChanged?.Invoke(true);
            Debug.Log("[GamePauseManager] Game Paused.");
        }

        /// <summary>
        /// Resumes the game if currently paused.
        /// Restores Time.timeScale to 1 and invokes OnPauseStateChanged(false).
        /// </summary>
        public void ResumeGame()
        {
            if (!_isPaused)
            {
                return;
            }

            _isPaused = false;
            Time.timeScale = 1f;
            OnPauseStateChanged?.Invoke(false);
            Debug.Log("[GamePauseManager] Game Resumed.");
        }

        /// <summary>
        /// Toggles between paused and running states.
        /// </summary>
        public void TogglePause()
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        /// <summary>
        /// Handles GameOver event: clears pause state without invoking pause UI.
        /// </summary>
        private void HandleGameOver()
        {
            if (_isPaused)
            {
                _isPaused = false;
                Time.timeScale = 1f;
                OnPauseStateChanged?.Invoke(false);
            }
        }
    }
}
