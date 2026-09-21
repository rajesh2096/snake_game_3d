using System;
using UnityEngine;
using SnakeGame3D.FoodSystem;

namespace SnakeGame3D.Game
{
    /// <summary>
    /// Manages the game score.
    /// Listens to Food.OnCollected events and increments score by 1.
    /// Exposes the current score and an OnScoreChanged event for UI systems.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [Header("Food Reference (Optional manual binding)")]
        [SerializeField] private Food _targetFood;

        [Header("Score State (Read-Only)")]
        [SerializeField] private int _currentScore = 0;

        /// <summary>
        /// Event fired whenever the score changes, passing the new score value.
        /// </summary>
        public event Action<int> OnScoreChanged;

        /// <summary>
        /// Gets the current game score.
        /// </summary>
        public int CurrentScore => _currentScore;

        public Food TargetFood
        {
            get => _targetFood;
            set
            {
                if (_targetFood != null)
                {
                    _targetFood.OnCollected -= HandleFoodCollected;
                }

                _targetFood = value;

                if (_targetFood != null && isActiveAndEnabled)
                {
                    _targetFood.OnCollected += HandleFoodCollected;
                }
            }
        }

        private void Awake()
        {
            _currentScore = 0;

            if (_targetFood == null)
            {
                _targetFood = FindAnyObjectByType<Food>();
            }
        }

        private void OnEnable()
        {
            if (_targetFood != null)
            {
                // Ensure no duplicate subscription before subscribing
                _targetFood.OnCollected -= HandleFoodCollected;
                _targetFood.OnCollected += HandleFoodCollected;
            }
        }

        private void OnDisable()
        {
            if (_targetFood != null)
            {
                _targetFood.OnCollected -= HandleFoodCollected;
            }
        }

        /// <summary>
        /// Resets the current score back to 0 and notifies listeners.
        /// </summary>
        public void ResetScore()
        {
            if (_currentScore != 0)
            {
                _currentScore = 0;
                OnScoreChanged?.Invoke(_currentScore);
            }
        }

        /// <summary>
        /// Increments score by 1 when food is collected.
        /// </summary>
        private void HandleFoodCollected()
        {
            AddScore(1);
        }

        /// <summary>
        /// Adds points to the current score and invokes OnScoreChanged.
        /// </summary>
        public void AddScore(int amount)
        {
            if (amount <= 0) return;

            _currentScore += amount;
            OnScoreChanged?.Invoke(_currentScore);
        }
    }
}
