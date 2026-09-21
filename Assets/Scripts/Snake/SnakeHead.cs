using UnityEngine;

namespace SnakeGame3D.Snake
{
    /// <summary>
    /// Controls the snake head's forward translation and smooth horizontal rotation.
    /// </summary>
    public class SnakeHead : MonoBehaviour
    {
        [Header("Movement Configuration")]
        [Tooltip("Forward movement speed in world units per second")]
        [SerializeField] private float _moveSpeed = 3.0f;

        [Tooltip("Turn rotation speed in degrees per second")]
        [SerializeField] private float _turnSpeed = 180.0f;

        private Vector3 _desiredDirection = Vector3.forward;

        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = Mathf.Max(0f, value);
        }

        public float TurnSpeed
        {
            get => _turnSpeed;
            set => _turnSpeed = Mathf.Max(0f, value);
        }

        public Vector3 CurrentDirection => transform.forward;
        public Vector3 DesiredDirection => _desiredDirection;

        /// <summary>
        /// Sets a new target horizontal direction for the head.
        /// </summary>
        public void SetDesiredDirection(Vector3 direction)
        {
            direction.y = 0f; // Constrain to ground plane

            if (direction.sqrMagnitude > 0.001f)
            {
                _desiredDirection = direction.normalized;
            }
        }

        /// <summary>
        /// Moves the head forward and rotates smoothly toward the desired direction.
        /// </summary>
        public void MoveHead(float deltaTime)
        {
            // 1. Smoothly rotate towards the desired direction
            if (_desiredDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_desiredDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * deltaTime);
            }

            // 2. Translate forward along the current facing direction
            Vector3 movement = transform.forward * (_moveSpeed * deltaTime);
            transform.position += movement;

            // Keep Y position stable above ground
            Vector3 pos = transform.position;
            pos.y = 0.5f;
            transform.position = pos;
        }

        /// <summary>
        /// Teleports the head to an initial state.
        /// </summary>
        public void ResetPosition(Vector3 position, Vector3 initialDirection)
        {
            position.y = 0.5f;
            transform.position = position;
            SetDesiredDirection(initialDirection);
            if (_desiredDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(_desiredDirection, Vector3.up);
            }
        }
    }
}
