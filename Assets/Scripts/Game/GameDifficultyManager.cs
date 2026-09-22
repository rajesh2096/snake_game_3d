using System;
using UnityEngine;
using SnakeGame3D.Snake;

namespace SnakeGame3D.Game
{
    public enum GameDifficulty
    {
        Easy = 0,
        Normal = 1,
        Hard = 2
    }

    /// <summary>
    /// Manages the game difficulty level (Easy, Normal, Hard) and sets the snake movement speed accordingly.
    /// Persists selected difficulty using PlayerPrefs under 'SnakeGame_Difficulty'.
    /// </summary>
    public class GameDifficultyManager : MonoBehaviour
    {
        public const string KeyDifficulty = "SnakeGame_Difficulty";

        private static GameDifficultyManager _instance;
        public static GameDifficultyManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<GameDifficultyManager>();
                }
                return _instance;
            }
        }

        [Header("Speed Configuration")]
        [SerializeField] private float _easySpeed = 2.25f;    // ~75% of 3.0
        [SerializeField] private float _normalSpeed = 3.0f;    // Default base speed
        [SerializeField] private float _hardSpeed = 3.75f;    // ~125% of 3.0

        [Header("Current State")]
        [SerializeField] private GameDifficulty _currentDifficulty = GameDifficulty.Normal;

        [Header("References")]
        [SerializeField] private SnakeController _snakeController;

        public event Action<GameDifficulty> OnDifficultyChanged;

        public GameDifficulty CurrentDifficulty
        {
            get => _currentDifficulty;
            set => SetDifficulty(value);
        }

        public float EasySpeed { get => _easySpeed; set => _easySpeed = value; }
        public float NormalSpeed { get => _normalSpeed; set => _normalSpeed = value; }
        public float HardSpeed { get => _hardSpeed; set => _hardSpeed = value; }

        public float CurrentSpeed => GetSpeedForDifficulty(_currentDifficulty);

        public SnakeController SnakeController
        {
            get => _snakeController;
            set => _snakeController = value;
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            LoadDifficulty();
        }

        private void Start()
        {
            ApplyDifficulty();
        }

        public float GetSpeedForDifficulty(GameDifficulty difficulty)
        {
            switch (difficulty)
            {
                case GameDifficulty.Easy:
                    return _easySpeed;
                case GameDifficulty.Hard:
                    return _hardSpeed;
                case GameDifficulty.Normal:
                default:
                    return _normalSpeed;
            }
        }

        public void SetDifficulty(GameDifficulty difficulty)
        {
            _currentDifficulty = difficulty;
            SaveDifficulty();
            ApplyDifficulty();
            OnDifficultyChanged?.Invoke(_currentDifficulty);
        }

        public void LoadDifficulty()
        {
            _currentDifficulty = (GameDifficulty)PlayerPrefs.GetInt(KeyDifficulty, (int)GameDifficulty.Normal);
        }

        public void SaveDifficulty()
        {
            PlayerPrefs.SetInt(KeyDifficulty, (int)_currentDifficulty);
            PlayerPrefs.Save();
        }

        public void ResetDifficulty()
        {
            SetDifficulty(GameDifficulty.Normal);
        }

        public void ApplyDifficulty()
        {
            if (_snakeController == null)
            {
                _snakeController = FindAnyObjectByType<SnakeController>();
            }

            if (_snakeController != null)
            {
                _snakeController.MovementSpeed = CurrentSpeed;
            }
        }
    }
}
