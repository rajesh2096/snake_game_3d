using System;
using UnityEngine;

namespace SnakeGame3D.InputSystem
{
    /// <summary>
    /// Lightweight touch and mouse swipe detection component.
    /// Detects touch begin, records start position, calculates delta on release,
    /// determines dominant axis, and raises a swipe event with a cardinal 3D world direction.
    /// Also supports optional keyboard input in Editor for testing.
    /// </summary>
    public class SwipeInput : MonoBehaviour
    {
        [Header("Swipe Configuration")]
        [Tooltip("Minimum swipe distance in screen pixels to register as a swipe")]
        [SerializeField] private float _minSwipeDistance = 50f;

        [Header("Editor Testing")]
        [Tooltip("Enable WASD / Arrow keys input in Unity Editor")]
        [SerializeField] private bool _enableKeyboardInEditor = true;

        private Vector2 _touchStartPosition;
        private bool _isSwiping;

        /// <summary>
        /// Event fired when a valid swipe or directional input is recognized.
        /// Provides the requested horizontal movement vector (Vector3.forward, Vector3.back, Vector3.left, Vector3.right).
        /// </summary>
        public event Action<Vector3> OnDirectionRequested;

        public float MinSwipeDistance
        {
            get => _minSwipeDistance;
            set => _minSwipeDistance = Mathf.Max(10f, value);
        }

        private void Update()
        {
            HandleTouchInput();
            HandleMouseSwipe();
            HandleKeyboardInput();
        }

        /// <summary>
        /// Native mobile touch input detection.
        /// </summary>
        private void HandleTouchInput()
        {
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchStartPosition = touch.position;
                    _isSwiping = true;
                    break;

                case TouchPhase.Ended:
                    if (_isSwiping)
                    {
                        ProcessSwipe(touch.position - _touchStartPosition);
                        _isSwiping = false;
                    }
                    break;

                case TouchPhase.Canceled:
                    _isSwiping = false;
                    break;
            }
        }

        /// <summary>
        /// Mouse click-and-drag swipe simulation for Editor & desktop testing.
        /// </summary>
        private void HandleMouseSwipe()
        {
            // If native touches exist, let touch handling take precedence
            if (Input.touchCount > 0) return;

            if (Input.GetMouseButtonDown(0))
            {
                _touchStartPosition = Input.mousePosition;
                _isSwiping = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_isSwiping)
                {
                    Vector2 delta = (Vector2)Input.mousePosition - _touchStartPosition;
                    ProcessSwipe(delta);
                    _isSwiping = false;
                }
            }
        }

        /// <summary>
        /// Determines dominant axis from swipe delta and broadcasts cardinal world direction.
        /// </summary>
        private void ProcessSwipe(Vector2 swipeDelta)
        {
            if (swipeDelta.magnitude < _minSwipeDistance)
            {
                return; // Below threshold
            }

            Vector3 requestedDirection;

            // Determine dominant axis (horizontal vs vertical)
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                // Horizontal swipe: Left or Right
                requestedDirection = swipeDelta.x > 0 ? Vector3.right : Vector3.left;
            }
            else
            {
                // Vertical swipe: Up or Down (mapped to 3D Z-axis forward/backward)
                requestedDirection = swipeDelta.y > 0 ? Vector3.forward : Vector3.back;
            }

            OnDirectionRequested?.Invoke(requestedDirection);
        }

        /// <summary>
        /// Editor keyboard testing fallback (W/A/S/D and Arrow keys).
        /// </summary>
        private void HandleKeyboardInput()
        {
            if (!_enableKeyboardInEditor && !Application.isEditor) return;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                OnDirectionRequested?.Invoke(Vector3.forward);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                OnDirectionRequested?.Invoke(Vector3.back);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnDirectionRequested?.Invoke(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnDirectionRequested?.Invoke(Vector3.right);
            }
        }
    }
}
