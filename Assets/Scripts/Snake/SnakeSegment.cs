using UnityEngine;

namespace SnakeGame3D.Snake
{
    /// <summary>
    /// Represents an individual body or tail segment following the sampled path behind the head.
    /// </summary>
    public class SnakeSegment : MonoBehaviour
    {
        [Tooltip("Distance in units behind the head that this segment should maintain")]
        [SerializeField] private float _targetDistanceBehindHead = 0.8f;

        public float TargetDistanceBehindHead
        {
            get => _targetDistanceBehindHead;
            set => _targetDistanceBehindHead = Mathf.Max(0f, value);
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
                transform.position = targetPosition;
                transform.rotation = targetRotation;
            }
        }
    }
}
