using UnityEngine;
using SnakeGame3D.Snake;
using SnakeGame3D.Gameplay;
using SnakeGame3D.FoodSystem;
using SnakeGame3D.UI;

namespace SnakeGame3D.Game
{
    /// <summary>
    /// Coordinates the restart flow for the 3D Snake Game.
    /// Resets GameOverManager, ScoreManager, SnakeController, ArenaBoundary, SnakeSelfCollision, Food, FoodSpawner, and GameOverUI
    /// in a deterministic sequence without reloading the scene.
    /// </summary>
    public class GameRestartManager : MonoBehaviour
    {
        [Header("System References")]
        [Tooltip("Reference to GameOverManager")]
        [SerializeField] private GameOverManager _gameOverManager;

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

        public GameOverManager GameOverManager { get => _gameOverManager; set => _gameOverManager = value; }
        public ScoreManager ScoreManager { get => _scoreManager; set => _scoreManager = value; }
        public SnakeController SnakeController { get => _snakeController; set => _snakeController = value; }
        public ArenaBoundary ArenaBoundary { get => _arenaBoundary; set => _arenaBoundary = value; }
        public SnakeSelfCollision SelfCollision { get => _selfCollision; set => _selfCollision = value; }
        public Food Food { get => _food; set => _food = value; }
        public FoodSpawner FoodSpawner { get => _foodSpawner; set => _foodSpawner = value; }
        public GameOverUI GameOverUI { get => _gameOverUI; set => _gameOverUI = value; }

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
            if (_scoreManager == null) _scoreManager = FindAnyObjectByType<ScoreManager>();
            if (_snakeController == null) _snakeController = FindAnyObjectByType<SnakeController>();
            if (_arenaBoundary == null) _arenaBoundary = FindAnyObjectByType<ArenaBoundary>();
            if (_selfCollision == null) _selfCollision = FindAnyObjectByType<SnakeSelfCollision>();
            if (_food == null) _food = FindAnyObjectByType<Food>();
            if (_foodSpawner == null) _foodSpawner = FindAnyObjectByType<FoodSpawner>();
            if (_gameOverUI == null) _gameOverUI = FindAnyObjectByType<GameOverUI>();
        }

        /// <summary>
        /// Executes full in-scene deterministic game restart:
        /// 1. Resets GameOverManager state
        /// 2. Resets ScoreManager (score back to 0)
        /// 3. Resets SnakeController (removes growth segments, resets position/direction/path, enables movement)
        /// 4. Resets ArenaBoundary state
        /// 5. Resets SnakeSelfCollision state
        /// 6. Resets Food and FoodSpawner state
        /// 7. Hides Game Over UI panel
        /// </summary>
        public void RestartGame()
        {
            ResolveReferences();

            // 1. Reset Game Over Manager
            if (_gameOverManager != null)
            {
                _gameOverManager.ResetGameOverState();
            }

            // 2. Reset Score Manager
            if (_scoreManager != null)
            {
                _scoreManager.ResetScore();
            }

            // 3. Reset Snake Controller
            if (_snakeController != null)
            {
                _snakeController.ResetSnake();
            }

            // 4. Reset Arena Boundary
            if (_arenaBoundary != null)
            {
                _arenaBoundary.ResetBoundaryState();
            }

            // 5. Reset Self Collision
            if (_selfCollision != null)
            {
                _selfCollision.ResetCollisionState();
            }

            // 6. Reset Food & FoodSpawner
            if (_foodSpawner != null)
            {
                _foodSpawner.RespawnFood();
            }
            else if (_food != null)
            {
                _food.ResetFood();
            }

            // 7. Hide Game Over UI
            if (_gameOverUI != null)
            {
                _gameOverUI.HidePanel();
            }

            Debug.Log("[GameRestartManager] Game successfully restarted in-scene.");
        }
    }
}
