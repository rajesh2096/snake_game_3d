using System;
using UnityEngine;
using SnakeGame3D.Snake;

namespace SnakeGame3D.FoodSystem
{
    /// <summary>
    /// Represents a 3D food item in the snake game.
    /// Detects when the SnakeHead enters its trigger collider and raises OnCollected event.
    /// Prevents duplicate collections and maintains a clean decoupled event-based architecture.
    /// </summary>
    public class Food : MonoBehaviour
    {
        [Header("Food Configuration")]
        [Tooltip("Fixed height above ground plane for food positioning")]
        [SerializeField] private float _groundYOffset = 0.4f;

        private bool _isCollected;
        private FoodVisualEffect _visualEffect;

        /// <summary>
        /// Event fired when the food item is collected by the snake head.
        /// </summary>
        public event Action OnCollected;

        public float GroundYOffset => _groundYOffset;
        public bool IsCollected => _isCollected;

        private void Awake()
        {
            // Ensure collider is configured as a trigger
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }

            _visualEffect = GetComponent<FoodVisualEffect>();
        }

        /// <summary>
        /// Repositions the food item at the given horizontal X/Z coordinate while keeping Y fixed.
        /// Resets the collected state so it can be collected again after repositioning.
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            position.y = _groundYOffset;
            transform.position = position;
            _isCollected = false;

            if (_visualEffect != null)
            {
                _visualEffect.ResetVisualState();
            }
        }

        /// <summary>
        /// Repositions the food item at the given horizontal coordinates.
        /// </summary>
        public void SetPosition(float x, float z)
        {
            SetPosition(new Vector3(x, _groundYOffset, z));
        }

        /// <summary>
        /// Resets the collection flag manually if needed and restores visual state.
        /// </summary>
        public void ResetFood()
        {
            _isCollected = false;
            if (_visualEffect != null)
            {
                _visualEffect.ResetVisualState();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isCollected) return; // Prevent duplicate detection

            // Specifically identify if the collider belongs to SnakeHead
            SnakeHead head = other.GetComponent<SnakeHead>() ?? other.GetComponentInParent<SnakeHead>();
            if (head != null)
            {
                _isCollected = true;
                Debug.Log("[Food] Snake head collected food.");
                OnCollected?.Invoke();
            }
        }
    }
}
