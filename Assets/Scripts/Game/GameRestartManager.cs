using UnityEngine;
using SnakeGame3D.Snake;
using SnakeGame3D.Gameplay;
using SnakeGame3D.FoodSystem;
using SnakeGame3D.UI;

namespace SnakeGame3D.Game
{
    /// <summary>
    /// Coordinates the restart flow for the 3D Snake Game.
    /// Resets GameOverManager, GamePauseManager, GameStartManager, ScoreManager, SnakeController, ArenaBoundary, SnakeSelfCollision, Food, FoodSpawner, GameOverUI, PauseUI, and GameStartUI
    /// in a deterministic sequence without reloading the scene.
    /// </summary>
    public class GameRestartManager : MonoBehaviour
    {
        [Header("System References")]
        [Tooltip("Reference to GameOverManager")]
        [SerializeField] private GameOverManager _gameOverManager;

        [Tooltip("Reference to GamePauseManager")]
        [SerializeField] private GamePauseManager _pauseManager;

        [Tooltip("Reference to GameStartManager")]
        [SerializeField] private GameStartManager _startManager;

        [Tooltip("Reference to ScoreManager")]
        [SerializeField] private ScoreManager _scoreManager;

        [Tooltip("Reference to SnakeController")]
        [SerializeField] private SnakeController _snakeController;

        [Tooltip("Reference to ArenaBoundary detector")]
        [SerializeField] private ArenaBoundary _arenaBoundary;

        [Tooltip("Reference to SnakeSelfCollision detector")]
        [SerializeField] private SnakeSelfCollision _selfCollision;

        [Tooltip("Reference to Food object")]
        [SerializeField] private Food _food;

        [Tooltip("Reference to FoodSpawner")]
        [SerializeField] private FoodSpawner _foodSpawner;

        [Tooltip("Reference to GameOverUI")]
        [SerializeField] private GameOverUI _gameOverUI;

        [Tooltip("Reference to PauseUI")]
        [SerializeField] private PauseUI _pauseUI;

        [Tooltip("Reference to GameStartUI")]
        [SerializeField] private GameStartUI _startUI;

        public GameOverManager GameOverManager { get => _gameOverManager; set => _gameOverManager = value; }
        public GamePauseManager PauseManager { get => _pauseManager; set => _pauseManager = value; }
        public GameStartManager StartManager { get => _startManager; set => _startManager = value; }
        public ScoreManager ScoreManager { get => _scoreManager; set => _scoreManager = value; }
        public SnakeController SnakeController { get => _snakeController; set => _snakeController = value; }
        public ArenaBoundary ArenaBoundary { get => _arenaBoundary; set => _arenaBoundary = value; }
        public SnakeSelfCollision SelfCollision { get => _selfCollision; set => _selfCollision = value; }
        public Food Food { get => _food; set => _food = value; }
        public FoodSpawner FoodSpawner { get => _foodSpawner; set => _foodSpawner = value; }
        public GameOverUI GameOverUI { get => _gameOverUI; set => _gameOverUI = value; }
        public PauseUI PauseUI { get => _pauseUI; set => _pauseUI = value; }
        public GameStartUI StartUI { get => _startUI; set => _startUI = value; }

        private void Awake()
        {
            ResolveReferences();
        }

        /// <summary>
        /// Automatically resolves unassigned references from the scene.
        /// </summary>
        public void ResolveReferences()
        {
            if (_gameOverManager == null) _gameOverManager = FindAnyObjectByType<GameOverManager>();
            if (_pauseManager == null) _pauseManager = FindAnyObjectByType<GamePauseManager>();
            if (_startManager == null) _startManager = FindAnyObjectByType<GameStartManager>();
            if (_scoreManager == null) _scoreManager = FindAnyObjectByType<ScoreManager>();
            if (_snakeController == null) _snakeController = FindAnyObjectByType<SnakeController>();
            if (_arenaBoundary == null) _arenaBoundary = FindAnyObjectByType<ArenaBoundary>();
            if (_selfCollision == null) _selfCollision = FindAnyObjectByType<SnakeSelfCollision>();
            if (_food == null) _food = FindAnyObjectByType<Food>();
            if (_foodSpawner == null) _foodSpawner = FindAnyObjectByType<FoodSpawner>();
            if (_gameOverUI == null) _gameOverUI = FindAnyObjectByType<GameOverUI>();
            if (_pauseUI == null) _pauseUI = FindAnyObjectByType<PauseUI>();
            if (_startUI == null) _startUI = FindAnyObjectByType<GameStartUI>();
        }

        /// <summary>
        /// Executes full in-scene deterministic game restart:
        /// 1. Resets GameOverManager state
        /// 2. Resets GamePauseManager (resumes if paused, timeScale = 1)
        /// 3. Resets GameStartManager (resets back to READY, movement disabled)
        /// 4. Resets ScoreManager (score back to 0)
        /// 5. Resets SnakeController (removes growth segments, resets position/direction/path, movement stopped)
        /// 6. Resets ArenaBoundary state
        /// 7. Resets SnakeSelfCollision state
        /// 8. Resets Food and FoodSpawner state
        /// 9. Hides Game Over UI panel, updates Pause UI & shows Start/Ready UI
        /// </summary>
        public void RestartGame()
        {
            ResolveReferences();

            // 1. Reset Game Over Manager
            if (_gameOverManager != null)
            {
                _gameOverManager.ResetGameOverState();
            }

            // 2. Reset Pause Manager
            if (_pauseManager != null)
            {
                _pauseManager.ResumeGame();
            }
            Time.timeScale = 1f;

            // 3. Reset Start Manager (returns game to READY state)
            if (_startManager != null)
            {
                _startManager.ResetStartState();
            }

            // 4. Reset Score Manager
            if (_scoreManager != null)
            {
                _scoreManager.ResetScore();
            }

            // 5. Reset Snake Controller & ensure movement stopped for READY state
            if (_snakeController != null)
            {
                _snakeController.ResetSnake();
                _snakeController.StopMovement();
            }

            // 6. Reset Arena Boundary
            if (_arenaBoundary != null)
            {
                _arenaBoundary.ResetBoundaryState();
            }

            // 7. Reset Self Collision
            if (_selfCollision != null)
            {
                _selfCollision.ResetCollisionState();
            }

            // 8. Reset Food & FoodSpawner
            if (_foodSpawner != null)
            {
                _foodSpawner.RespawnFood();
            }
            else if (_food != null)
            {
                _food.ResetFood();
            }

            // 9. Update UI panels
            if (_gameOverUI != null)
            {
                _gameOverUI.HidePanel();
            }

            if (_startUI != null)
            {
                _startUI.UpdateUIState(false);
            }

            if (_pauseUI != null)
            {
                _pauseUI.UpdateUIState(false);
            }

            Debug.Log("[GameRestartManager] Game successfully restarted in-scene to READY state.");
        }
    }
}
