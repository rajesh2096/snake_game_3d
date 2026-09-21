using System;
using System.Collections.Generic;
using UnityEngine;
using SnakeGame3D.Snake;

namespace SnakeGame3D.Gameplay
{
    /// <summary>
    /// Detects continuous 3D self-collision between the Snake Head and its body/tail segments.
    /// Excludes the nearest segments behind the head to prevent false positives during natural movement.
    /// Exposes read-only collision state and fires OnSelfCollision once per collision event.
    /// </summary>
    public class SnakeSelfCollision : MonoBehaviour
    {
        [Header("Target Tracking")]
        [Tooltip("SnakeController owning the head and body segments")]
        [SerializeField] private SnakeController _snakeController;

        [Header("Collision Parameters")]
        [Tooltip("World distance threshold between head and segment center to register collision")]
        [SerializeField] private float _collisionThreshold = 0.65f;

        [Tooltip("Number of leading body segments behind head to ignore (prevents immediate false self-collisions)")]
        [SerializeField] private int _ignoredLeadingSegments = 2;

        [Header("State (Read-Only)")]
        [SerializeField] private bool _isSelfCollided = false;

        /// <summary>
        /// Event fired once when the snake head collides with any valid body or tail segment.
        /// </summary>
        public event Action OnSelfCollision;

        /// <summary>
        /// Event fired once when the snake head clears collision with its body.
        /// </summary>
        public event Action OnSelfCollisionCleared;

        public SnakeController SnakeController
        {
            get => _snakeController;
            set => _snakeController = value;
        }

        public float CollisionThreshold
        {
            get => _collisionThreshold;
            set => _collisionThreshold = Mathf.Max(0.01f, value);
        }

        public int IgnoredLeadingSegments
        {
            get => _ignoredLeadingSegments;
            set => _ignoredLeadingSegments = Mathf.Max(0, value);
        }

        public bool IsSelfCollided => _isSelfCollided;

        private void Awake()
        {
            if (_snakeController == null)
            {
                _snakeController = GetComponent<SnakeController>() ?? FindAnyObjectByType<SnakeController>();
            }
        }

        private void Update()
        {
            if (_snakeController == null || _snakeController.Head == null)
            {
                return;
            }

            CheckSelfCollision(_snakeController.Head.transform.position, _snakeController.Segments);
        }

        /// <summary>
        /// Evaluates whether the given head position collides with any non-ignored body segments.
        /// Triggers OnSelfCollision or OnSelfCollisionCleared upon state transitions.
        /// </summary>
        public void CheckSelfCollision(Vector3 headPosition, IReadOnlyList<SnakeSegment> segments)
        {
            if (segments == null || segments.Count <= _ignoredLeadingSegments)
            {
                if (_isSelfCollided)
                {
                    _isSelfCollided = false;
                    OnSelfCollisionCleared?.Invoke();
                }
                return;
            }

            bool collided = false;
            float sqrThreshold = _collisionThreshold * _collisionThreshold;

            // Check against segments starting after ignored count
            for (int i = _ignoredLeadingSegments; i < segments.Count; i++)
            {
                SnakeSegment seg = segments[i];
                if (seg == null) continue;

                Vector3 segPos = seg.transform.position;
                // Evaluate horizontal X/Z world distance (ignore small vertical variance)
                float dx = headPosition.x - segPos.x;
                float dz = headPosition.z - segPos.z;
                float sqrDist = dx * dx + dz * dz;

                if (sqrDist <= sqrThreshold)
                {
                    collided = true;
                    break;
                }
            }

            if (collided && !_isSelfCollided)
            {
                // Transition: Clear -> Collided
                _isSelfCollided = true;
                OnSelfCollision?.Invoke();
            }
            else if (!collided && _isSelfCollided)
            {
                // Transition: Collided -> Clear
                _isSelfCollided = false;
                OnSelfCollisionCleared?.Invoke();
            }
        }

        /// <summary>
        /// Manually resets the self-collision state.
        /// </summary>
        public void ResetCollisionState()
        {
            _isSelfCollided = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (_snakeController != null && _snakeController.Head != null)
            {
                Gizmos.color = _isSelfCollided ? Color.red : Color.cyan;
                Gizmos.DrawWireSphere(_snakeController.Head.transform.position, _collisionThreshold);
            }
        }
    }
}
