using System;
using UnityEngine;
using SnakeGame3D.Snake;

namespace SnakeGame3D.Gameplay
{
    /// <summary>
    /// Monitors the 3D playfield bounds (world-space X/Z) and detects when the Snake head exits the playable arena.
    /// Broadcasts an event upon the first exit transition and exposes current out-of-bounds state.
    /// Configured by default for the centered 100m x 100m ground plane.
    /// </summary>
    public class ArenaBoundary : MonoBehaviour
    {
        [Header("Target Tracking")]
        [Tooltip("Transform of the Snake Head to monitor")]
        [SerializeField] private Transform _targetHead;

        [Header("Playable Arena Bounds (World Coordinates)")]
        [Tooltip("Minimum X coordinate of playable area")]
        [SerializeField] private float _minX = -50f;

        [Tooltip("Maximum X coordinate of playable area")]
        [SerializeField] private float _maxX = 50f;

        [Tooltip("Minimum Z coordinate of playable area")]
        [SerializeField] private float _minZ = -50f;

        [Tooltip("Maximum Z coordinate of playable area")]
        [SerializeField] private float _maxZ = 50f;

        [Header("Safety Margin")]
        [Tooltip("Optional offset inwards (+) or outwards (-) from boundary edges")]
        [SerializeField] private float _boundaryMargin = 0f;

        [Header("State (Read-Only)")]
        [SerializeField] private bool _isOutOfBounds = false;

        /// <summary>
        /// Event fired once when the snake head transitions from inside to outside the boundary.
        /// </summary>
        public event Action OnBoundaryExited;

        /// <summary>
        /// Event fired once when the snake head returns from outside back inside the boundary.
        /// </summary>
        public event Action OnBoundaryEntered;

        public Transform TargetHead
        {
            get => _targetHead;
            set => _targetHead = value;
        }

        public float MinX { get => _minX; set => _minX = value; }
        public float MaxX { get => _maxX; set => _maxX = value; }
        public float MinZ { get => _minZ; set => _minZ = value; }
        public float MaxZ { get => _maxZ; set => _maxZ = value; }
        public float BoundaryMargin { get => _boundaryMargin; set => _boundaryMargin = value; }

        public bool IsOutOfBounds => _isOutOfBounds;

        public float EffectiveMinX => _minX + _boundaryMargin;
        public float EffectiveMaxX => _maxX - _boundaryMargin;
        public float EffectiveMinZ => _minZ + _boundaryMargin;
        public float EffectiveMaxZ => _maxZ - _boundaryMargin;

        private void Awake()
        {
            if (_targetHead == null)
            {
                SnakeHead head = FindAnyObjectByType<SnakeHead>();
                if (head != null)
                {
                    _targetHead = head.transform;
                }
            }
        }

        private void Update()
        {
            if (_targetHead == null)
            {
                return;
            }

            CheckBoundaryState(_targetHead.position);
        }

        /// <summary>
        /// Checks a given world-space position against the arena bounds and triggers transition events.
        /// </summary>
        public void CheckBoundaryState(Vector3 position)
        {
            bool outNow = IsPositionOutOfBounds(position);

            if (outNow && !_isOutOfBounds)
            {
                // Transitioned: Inside -> Outside
                _isOutOfBounds = true;
                OnBoundaryExited?.Invoke();
            }
            else if (!outNow && _isOutOfBounds)
            {
                // Transitioned: Outside -> Inside (e.g. Reset/Reposition)
                _isOutOfBounds = false;
                OnBoundaryEntered?.Invoke();
            }
        }

        /// <summary>
        /// Evaluates whether a world-space position is outside the effective rectangular bounds (X/Z plane).
        /// </summary>
        public bool IsPositionOutOfBounds(Vector3 position)
        {
            return position.x < EffectiveMinX ||
                   position.x > EffectiveMaxX ||
                   position.z < EffectiveMinZ ||
                   position.z > EffectiveMaxZ;
        }

        /// <summary>
        /// Resets the out-of-bounds state flag back to false.
        /// </summary>
        public void ResetBoundaryState()
        {
            _isOutOfBounds = false;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw playable boundary in editor scene view
            Gizmos.color = _isOutOfBounds ? Color.red : Color.yellow;
            float width = EffectiveMaxX - EffectiveMinX;
            float length = EffectiveMaxZ - EffectiveMinZ;
            Vector3 center = new Vector3((EffectiveMinX + EffectiveMaxX) * 0.5f, 0.5f, (EffectiveMinZ + EffectiveMaxZ) * 0.5f);
            Gizmos.DrawWireCube(center, new Vector3(width, 1.0f, length));
        }
    }
}
