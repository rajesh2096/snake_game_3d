using UnityEngine;

namespace SnakeGame3D.Cameras
{
    /// <summary>
    /// Smooth 3D perspective camera-follow controller for the snake.
    /// Follows the snake head target with configurable offset, smooth damping for translation,
    /// and smooth rotation towards the target.
    /// </summary>
    public class SnakeCameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("The Transform to follow (typically Snake Head)")]
        [SerializeField] private Transform _target;

        [Header("Follow Configuration")]
        [Tooltip("Offset relative to the target in world space")]
        [SerializeField] private Vector3 _followOffset = new Vector3(0f, 18f, -14f);

        [Tooltip("Smooth time for position damping in seconds")]
        [SerializeField] private float _followSmoothTime = 0.25f;

        [Header("Look Configuration")]
        [Tooltip("Offset added to target position for look direction")]
        [SerializeField] private Vector3 _lookOffset = new Vector3(0f, 0.5f, 2f);

        [Tooltip("Rotation interpolation speed")]
        [SerializeField] private float _lookSmoothSpeed = 10f;

        private Vector3 _currentVelocity;

        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        public Vector3 FollowOffset
        {
            get => _followOffset;
            set => _followOffset = value;
        }

        public float FollowSmoothTime
        {
            get => _followSmoothTime;
            set => _followSmoothTime = Mathf.Max(0.01f, value);
        }

        public float LookSmoothSpeed
        {
            get => _lookSmoothSpeed;
            set => _lookSmoothSpeed = Mathf.Max(0.1f, value);
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            // 1. Calculate desired camera position
            Vector3 targetPosition = _target.position;
            Vector3 desiredPosition = targetPosition + _followOffset;

            // 2. Smoothly damp camera position toward desired position
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref _currentVelocity,
                _followSmoothTime
            );

            // 3. Smoothly rotate camera toward look point near snake head
            Vector3 lookTarget = targetPosition + _lookOffset;
            Vector3 lookDirection = lookTarget - transform.position;

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    _lookSmoothSpeed * Time.deltaTime
                );
            }
        }

        /// <summary>
        /// Instantly snaps the camera to its target position without damping (e.g. on game start / reset).
        /// </summary>
        public void SnapToTarget()
        {
            if (_target == null) return;

            transform.position = _target.position + _followOffset;
            _currentVelocity = Vector3.zero;

            Vector3 lookTarget = _target.position + _lookOffset;
            Vector3 lookDirection = lookTarget - transform.position;
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            }
        }
    }
}
