using UnityEngine;

namespace SnakeGame3D.Snake
{
    /// <summary>
    /// Represents an individual body or tail segment following the sampled path behind the head.
    /// Provides smooth interpolation for visual polish without affecting gameplay path sampling.
    /// </summary>
    public class SnakeSegment : MonoBehaviour
    {
        [Tooltip("Distance in units behind the head that this segment should maintain")]
        [SerializeField] private float _targetDistanceBehindHead = 0.8f;

        [Tooltip("Smooth rotation interpolation speed (0 for instant snapping)")]
        [SerializeField] private float _rotationSmoothSpeed = 25.0f;

        [Tooltip("Smooth position interpolation speed (0 for instant snapping)")]
        [SerializeField] private float _positionSmoothSpeed = 30.0f;

        public float TargetDistanceBehindHead
        {
            get => _targetDistanceBehindHead;
            set => _targetDistanceBehindHead = Mathf.Max(0f, value);
        }

        public float RotationSmoothSpeed
        {
            get => _rotationSmoothSpeed;
            set => _rotationSmoothSpeed = Mathf.Max(0f, value);
        }

        public float PositionSmoothSpeed
        {
            get => _positionSmoothSpeed;
            set => _positionSmoothSpeed = Mathf.Max(0f, value);
        }

        /// <summary>
        /// Updates the segment's transform by sampling the recorded path.
        /// </summary>
        public void UpdatePositionFromPath(SnakePath path)
        {
            if (path == null)
            {
                return;
            }

            if (path.SamplePathAtDistance(_targetDistanceBehindHead, out Vector3 targetPosition, out Quaternion targetRotation))
            {
                // Constrain Y position to align with ground level
                targetPosition.y = 0.5f;

                if (_positionSmoothSpeed > 0f && Time.deltaTime > 0f)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, _positionSmoothSpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = targetPosition;
                }

                if (_rotationSmoothSpeed > 0f && Time.deltaTime > 0f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothSpeed * Time.deltaTime);
                }
                else
                {
                    transform.rotation = targetRotation;
                }
            }
        }

        /// <summary>
        /// Immediately snaps the segment position and rotation to the path without interpolation.
        /// Useful during initialization and restart.
        /// </summary>
        public void SnapToPath(SnakePath path)
        {
            if (path == null) return;

            if (path.SamplePathAtDistance(_targetDistanceBehindHead, out Vector3 targetPosition, out Quaternion targetRotation))
            {
                targetPosition.y = 0.5f;
                transform.position = targetPosition;
                transform.rotation = targetRotation;
            }
        }
    }
}
