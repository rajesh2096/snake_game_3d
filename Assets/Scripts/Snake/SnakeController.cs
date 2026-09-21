using System.Collections.Generic;
using UnityEngine;
using SnakeGame3D.InputSystem;

namespace SnakeGame3D.Snake
{
    /// <summary>
    /// Master coordinator for the continuous 3D Snake.
    /// Manages the head, path history, body segments, and directional inputs.
    /// Listens to SwipeInput events and applies 180-degree reverse-turn protection.
    /// </summary>
    public class SnakeController : MonoBehaviour
    {
        [Header("Snake Components")]
        [SerializeField] private SnakeHead _head;
        [SerializeField] private Transform _bodyContainer;
        [SerializeField] private List<SnakeSegment> _segments = new List<SnakeSegment>();
        [SerializeField] private SwipeInput _swipeInput;

        [Header("Movement & Turning Configuration")]
        [Tooltip("Forward movement speed in units per second")]
        [SerializeField] private float _movementSpeed = 3.0f;

        [Tooltip("Turn speed in degrees per second")]
        [SerializeField] private float _turnSpeed = 180.0f;

        [Tooltip("Spacing between adjacent segments in world units")]
        [SerializeField] private float _segmentSpacing = 0.8f;

        [Header("Initial Setup")]
        [SerializeField] private Vector3 _initialPosition = new Vector3(0f, 0.5f, 0f);
        [SerializeField] private Vector3 _initialDirection = Vector3.forward;

        private SnakePath _snakePath;

        public float MovementSpeed
        {
            get => _movementSpeed;
            set
            {
                _movementSpeed = Mathf.Max(0f, value);
                if (_head != null) _head.MoveSpeed = _movementSpeed;
            }
        }

        public float TurnSpeed
        {
            get => _turnSpeed;
            set
            {
                _turnSpeed = Mathf.Max(0f, value);
                if (_head != null) _head.TurnSpeed = _turnSpeed;
            }
        }

        public float SegmentSpacing
        {
            get => _segmentSpacing;
            set
            {
                _segmentSpacing = Mathf.Max(0.1f, value);
                UpdateSegmentDistances();
            }
        }

        public SnakeHead Head => _head;
        public IReadOnlyList<SnakeSegment> Segments => _segments;

        private void Awake()
        {
            _snakePath = new SnakePath(minRecordDistance: 0.05f);

            if (_head == null)
            {
                _head = GetComponentInChildren<SnakeHead>();
            }

            if (_swipeInput == null)
            {
                _swipeInput = GetComponent<SwipeInput>() ?? GetComponentInChildren<SwipeInput>();
            }

            // Auto-collect segments from body container if not assigned
            if (_segments.Count == 0 && _bodyContainer != null)
            {
                _segments.AddRange(_bodyContainer.GetComponentsInChildren<SnakeSegment>());
            }
        }

        private void OnEnable()
        {
            if (_swipeInput != null)
            {
                _swipeInput.OnDirectionRequested += OnDirectionInput;
            }
        }

        private void OnDisable()
        {
            if (_swipeInput != null)
            {
                _swipeInput.OnDirectionRequested -= OnDirectionInput;
            }
        }

        private void Start()
        {
            InitializeSnake();
        }

        public void InitializeSnake()
        {
            if (_head == null)
            {
                Debug.LogWarning("[SnakeController] SnakeHead reference is missing!");
                return;
            }

            // Apply speed settings
            _head.MoveSpeed = _movementSpeed;
            _head.TurnSpeed = _turnSpeed;

            // Reset head position
            _head.ResetPosition(_initialPosition, _initialDirection);

            // Configure path history and seed initial trail
            _snakePath.Reset(_head.transform.position, _head.transform.rotation);

            // Seed path backward so body segments align smoothly on start
            float totalLength = (_segments.Count + 1) * _segmentSpacing;
            Vector3 backwardDir = -_initialDirection.normalized;
            for (float d = 0.1f; d <= totalLength + 1f; d += 0.1f)
            {
                Vector3 seedPos = _initialPosition + backwardDir * d;
                _snakePath.UpdateHeadPosition(seedPos, Quaternion.LookRotation(_initialDirection, Vector3.up), totalLength + 5f);
            }
            _snakePath.UpdateHeadPosition(_initialPosition, Quaternion.LookRotation(_initialDirection, Vector3.up), totalLength + 5f);

            UpdateSegmentDistances();
            UpdateBodyPositions();
        }

        private void Update()
        {
            // Advance head forward and turn
            if (_head != null)
            {
                _head.MoveHead(Time.deltaTime);

                // Record head trajectory into SnakePath
                float maxRequiredDistance = (_segments.Count + 1) * _segmentSpacing;
                _snakePath.UpdateHeadPosition(_head.transform.position, _head.transform.rotation, maxRequiredDistance);

                // Update all body segments along the recorded path
                UpdateBodyPositions();
            }
        }

        /// <summary>
        /// Callback when a direction is requested via swipe or keyboard.
        /// Validates against 180-degree direct reverse before setting.
        /// </summary>
        private void OnDirectionInput(Vector3 requestedDirection)
        {
            SetDirection(requestedDirection);
        }

        /// <summary>
        /// Public API to set desired horizontal snake movement direction.
        /// Respects 180-degree reverse-direction protection.
        /// </summary>
        public void SetDirection(Vector3 direction)
        {
            if (_head == null) return;

            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) return;

            Vector3 normalizedDir = direction.normalized;

            // 180-degree reversal check:
            // Compare against current facing direction and currently target direction.
            // Dot product close to -1.0 means opposite direction (180 degrees).
            Vector3 currentDir = _head.CurrentDirection;
            Vector3 targetDir = _head.DesiredDirection;

            float dotCurrent = Vector3.Dot(currentDir, normalizedDir);
            float dotTarget = Vector3.Dot(targetDir, normalizedDir);

            // If the snake has body segments, reverse direction is prohibited
            if (_segments.Count > 0 && (dotCurrent < -0.7f || dotTarget < -0.7f))
            {
                // Illegal 180-degree reverse ignored
                return;
            }

            _head.SetDesiredDirection(normalizedDir);
        }

        private void UpdateSegmentDistances()
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                if (_segments[i] != null)
                {
                    _segments[i].TargetDistanceBehindHead = (i + 1) * _segmentSpacing;
                }
            }
        }

        private void UpdateBodyPositions()
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                if (_segments[i] != null)
                {
                    _segments[i].UpdatePositionFromPath(_snakePath);
                }
            }
        }
    }
}
