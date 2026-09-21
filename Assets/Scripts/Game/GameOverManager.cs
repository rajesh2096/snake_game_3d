using System;
using UnityEngine;
using SnakeGame3D.Gameplay;

namespace SnakeGame3D.Game
{
    /// <summary>
    /// Manages the Game Over state for the 3D Snake Game.
    /// Listens to snake self-collision and arena boundary exit events.
    /// Ensures OnGameOver is raised exactly once per game-over transition.
    /// </summary>
    public class GameOverManager : MonoBehaviour
    {
        [Header("Event Sources (Optional manual binding)")]
        [Tooltip("Self collision detector component to monitor")]
        [SerializeField] private SnakeSelfCollision _selfCollision;

        [Tooltip("Arena boundary detector component to monitor")]
        [SerializeField] private ArenaBoundary _arenaBoundary;

        [Header("State (Read-Only)")]
        [SerializeField] private bool _isGameOver = false;

        /// <summary>
        /// Event raised once when the game enters the Game Over state.
        /// </summary>
        public event Action OnGameOver;

        /// <summary>
        /// Gets whether the game is currently in the Game Over state.
        /// </summary>
        public bool IsGameOver => _isGameOver;

        public SnakeSelfCollision SelfCollision
        {
            get => _selfCollision;
            set
            {
                if (_selfCollision != null)
                {
                    _selfCollision.OnSelfCollision -= HandleSelfCollision;
                }

                _selfCollision = value;

                if (_selfCollision != null && isActiveAndEnabled)
                {
                    _selfCollision.OnSelfCollision += HandleSelfCollision;
                }
            }
        }

        public ArenaBoundary ArenaBoundary
        {
            get => _arenaBoundary;
            set
            {
                if (_arenaBoundary != null)
                {
                    _arenaBoundary.OnBoundaryExited -= HandleBoundaryExited;
                }

                _arenaBoundary = value;

                if (_arenaBoundary != null && isActiveAndEnabled)
                {
                    _arenaBoundary.OnBoundaryExited += HandleBoundaryExited;
                }
            }
        }

        private void Awake()
        {
            _isGameOver = false;

            if (_selfCollision == null)
            {
                _selfCollision = FindAnyObjectByType<SnakeSelfCollision>();
            }

            if (_arenaBoundary == null)
            {
                _arenaBoundary = FindAnyObjectByType<ArenaBoundary>();
            }
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (_selfCollision != null)
            {
                _selfCollision.OnSelfCollision -= HandleSelfCollision;
                _selfCollision.OnSelfCollision += HandleSelfCollision;
            }

            if (_arenaBoundary != null)
            {
                _arenaBoundary.OnBoundaryExited -= HandleBoundaryExited;
                _arenaBoundary.OnBoundaryExited += HandleBoundaryExited;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_selfCollision != null)
            {
                _selfCollision.OnSelfCollision -= HandleSelfCollision;
            }

            if (_arenaBoundary != null)
            {
                _arenaBoundary.OnBoundaryExited -= HandleBoundaryExited;
            }
        }

        private void HandleSelfCollision()
        {
            TriggerGameOver();
        }

        private void HandleBoundaryExited()
        {
            TriggerGameOver();
        }

        /// <summary>
        /// Transitions the state to Game Over and fires OnGameOver once.
        /// Subsequent calls while already Game Over are ignored.
        /// </summary>
        public void TriggerGameOver()
        {
            if (_isGameOver)
            {
                return;
            }

            _isGameOver = true;
            OnGameOver?.Invoke();
        }

        /// <summary>
        /// Resets the Game Over state back to false.
        /// </summary>
        public void ResetGameOverState()
        {
            _isGameOver = false;
        }
    }
}
