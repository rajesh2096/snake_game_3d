using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace SnakeGame3D.InputSystem
{
    /// <summary>
    /// Touch, mouse, and keyboard swipe detection component powered by the New Input System.
    /// Detects touch begin, records start position, calculates delta on release,
    /// determines dominant axis, and raises a swipe event with a cardinal 3D world direction.
    /// Fully compliant with activeInputHandler: 1 (New Input System only).
    /// </summary>
    public class SwipeInput : MonoBehaviour
    {
        [Header("Swipe Configuration")]
        [Tooltip("Minimum swipe distance in screen pixels to register as a swipe")]
        [SerializeField] private float _minSwipeDistance = 50f;

        [Header("Editor / Desktop Testing")]
        [Tooltip("Enable WASD / Arrow keys input in Unity Editor and Desktop")]
        [SerializeField] private bool _enableKeyboard = true;

        [Tooltip("Enable mouse drag swipe simulation for Editor and Desktop")]
        [SerializeField] private bool _enableMouseSimulation = true;

        private Vector2 _touchStartPosition;
        private bool _isSwiping;
        private int _activeTouchFingerId = -1;

        private Vector2 _mouseStartPosition;
        private bool _isMouseSwiping;

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

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
            _isSwiping = false;
            _isMouseSwiping = false;
            _activeTouchFingerId = -1;
        }

        private void Update()
        {
            HandleEnhancedTouchInput();
            HandleNewInputSystemMouse();
            HandleNewInputSystemKeyboard();
        }

        /// <summary>
        /// Touch input detection using New Input System EnhancedTouch.
        /// </summary>
        private void HandleEnhancedTouchInput()
        {
            var touches = Touch.activeTouches;
            if (touches.Count == 0)
            {
                if (_isSwiping)
                {
                    _isSwiping = false;
                    _activeTouchFingerId = -1;
                }
                return;
            }

            // Track the primary active touch finger
            Touch primaryTouch = default;
            bool foundPrimary = false;

            if (_activeTouchFingerId != -1)
            {
                for (int i = 0; i < touches.Count; i++)
                {
                    if (touches[i].finger.index == _activeTouchFingerId)
                    {
                        primaryTouch = touches[i];
                        foundPrimary = true;
                        break;
                    }
                }
            }

            if (!foundPrimary)
            {
                primaryTouch = touches[0];
            }

            switch (primaryTouch.phase)
            {
                case TouchPhase.Began:
                    _touchStartPosition = primaryTouch.screenPosition;
                    _isSwiping = true;
                    _activeTouchFingerId = primaryTouch.finger.index;
                    break;

                case TouchPhase.Ended:
                    if (_isSwiping && primaryTouch.finger.index == _activeTouchFingerId)
                    {
                        Vector2 delta = primaryTouch.screenPosition - _touchStartPosition;
                        ProcessSwipe(delta);
                        _isSwiping = false;
                        _activeTouchFingerId = -1;
                    }
                    break;

                case TouchPhase.Canceled:
                    _isSwiping = false;
                    _activeTouchFingerId = -1;
                    break;
            }
        }

        /// <summary>
        /// Mouse click-and-drag swipe simulation using New Input System Mouse.current.
        /// </summary>
        private void HandleNewInputSystemMouse()
        {
            if (!_enableMouseSimulation) return;
            if (Touch.activeTouches.Count > 0) return; // Touch takes priority

            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                _mouseStartPosition = mouse.position.ReadValue();
                _isMouseSwiping = true;
            }
            else if (mouse.leftButton.wasReleasedThisFrame)
            {
                if (_isMouseSwiping)
                {
                    Vector2 currentPos = mouse.position.ReadValue();
                    Vector2 delta = currentPos - _mouseStartPosition;
                    ProcessSwipe(delta);
                    _isMouseSwiping = false;
                }
            }
        }

        /// <summary>
        /// Keyboard directional input using New Input System Keyboard.current (W/A/S/D and Arrow keys).
        /// </summary>
        private void HandleNewInputSystemKeyboard()
        {
            if (!_enableKeyboard) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
            {
                OnDirectionRequested?.Invoke(Vector3.forward);
            }
            else if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
            {
                OnDirectionRequested?.Invoke(Vector3.back);
            }
            else if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
            {
                OnDirectionRequested?.Invoke(Vector3.left);
            }
            else if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
            {
                OnDirectionRequested?.Invoke(Vector3.right);
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
    }
}