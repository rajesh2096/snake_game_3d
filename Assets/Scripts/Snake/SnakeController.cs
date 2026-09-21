using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame3D.Snake
{
    /// <summary>
    /// Master coordinator for the continuous 3D Snake.
    /// Manages the head, path history, body segments, and directional inputs.
    /// </summary>
    public class SnakeController : MonoBehaviour
    {
        [Header("Snake Components")]
        [SerializeField] private SnakeHead _head;
        [SerializeField] private Transform _bodyContainer;
        [SerializeField] private List<SnakeSegment> _segments = new List<SnakeSegment>();

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

            // Auto-collect segments from body container if not assigned
            if (_segments.Count == 0 && _bodyContainer != null)
            {
                _segments.AddRange(_bodyContainer.GetComponentsInChildren<SnakeSegment>());
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
            // 1. Temporary keyboard input for movement validation (W/A/S/D or Arrow keys)
            HandleTestingInput();

            // 2. Advance head forward and turn
            if (_head != null)
            {
                _head.MoveHead(Time.deltaTime);

                // 3. Record head trajectory into SnakePath
                float maxRequiredDistance = (_segments.Count + 1) * _segmentSpacing;
                _snakePath.UpdateHeadPosition(_head.transform.position, _head.transform.rotation, maxRequiredDistance);

                // 4. Update all body segments along the recorded path
                UpdateBodyPositions();
            }
        }

        /// <summary>
        /// Public API to set desired horizontal snake movement direction.
        /// Touch/swipe controllers can call this directly.
        /// </summary>
        public void SetDirection(Vector3 direction)
        {
            if (_head != null)
            {
                _head.SetDesiredDirection(direction);
            }
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

        /// <summary>
        /// Lightweight temporary input mapping for testing movement & turning in Editor.
        /// </summary>
        private void HandleTestingInput()
        {
            Vector3 inputDir = Vector3.zero;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                inputDir += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                inputDir += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                inputDir += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                inputDir += Vector3.right;
            }

            if (inputDir.sqrMagnitude > 0.001f)
            {
                SetDirection(inputDir.normalized);
            }
        }
    }
}
