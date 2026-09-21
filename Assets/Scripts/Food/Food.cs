using UnityEngine;

namespace SnakeGame3D.FoodSystem
{
    /// <summary>
    /// Represents a 3D food item in the snake game.
    /// Provides component identity and position helper methods for future collection/spawning steps.
    /// </summary>
    public class Food : MonoBehaviour
    {
        [Header("Food Configuration")]
        [Tooltip("Fixed height above ground plane for food positioning")]
        [SerializeField] private float _groundYOffset = 0.4f;

        public float GroundYOffset => _groundYOffset;

        /// <summary>
        /// Repositions the food item at the given horizontal X/Z coordinate while keeping Y fixed.
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            position.y = _groundYOffset;
            transform.position = position;
        }

        /// <summary>
        /// Repositions the food item at the given horizontal coordinates.
        /// </summary>
        public void SetPosition(float x, float z)
        {
            transform.position = new Vector3(x, _groundYOffset, z);
        }
    }
}
