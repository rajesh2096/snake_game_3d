using UnityEngine;
using SnakeGame3D.Snake;
using SnakeGame3D.Gameplay;

namespace SnakeGame3D.FoodSystem
{
    /// <summary>
    /// Coordinates safe random food placement in the arena.
    /// Listens to Food.OnCollected events and repositions the existing Food instance to a new safe location.
    /// Ensures food never spawns on top of the snake head or body segments and respects arena margins.
    /// </summary>
    public class FoodSpawner : MonoBehaviour
    {
        [Header("Target References")]
        [Tooltip("Food instance to reposition")]
        [SerializeField] private Food _food;

        [Tooltip("SnakeController used to query snake head and segment positions")]
        [SerializeField] private SnakeController _snakeController;

        [Tooltip("ArenaBoundary used to query arena bounds")]
        [SerializeField] private ArenaBoundary _arenaBoundary;

        [Header("Spawn Configuration")]
        [Tooltip("Safety margin inset from the arena boundaries")]
        [SerializeField] private float _spawnMargin = 5f;

        [Tooltip("Minimum horizontal distance from snake head or body segments")]
        [SerializeField] private float _minimumSnakeDistance = 2.0f;

        [Tooltip("Maximum random placement attempts before choosing fallback")]
        [SerializeField] private int _maxSpawnAttempts = 50;

        [Tooltip("Fixed height above ground for food item")]
        [SerializeField] private float _spawnHeight = 0.4f;

        public Food Food
        {
            get => _food;
            set
            {
                if (_food != null)
                {
                    _food.OnCollected -= HandleFoodCollected;
                }
                _food = value;
                if (_food != null && isActiveAndEnabled)
                {
                    _food.OnCollected += HandleFoodCollected;
                }
            }
        }

        public SnakeController SnakeController
        {
            get => _snakeController;
            set => _snakeController = value;
        }

        public ArenaBoundary ArenaBoundary
        {
            get => _arenaBoundary;
            set => _arenaBoundary = value;
        }

        public float SpawnMargin
        {
            get => _spawnMargin;
            set => _spawnMargin = Mathf.Max(0f, value);
        }

        public float MinimumSnakeDistance
        {
            get => _minimumSnakeDistance;
            set => _minimumSnakeDistance = Mathf.Max(0.1f, value);
        }

        public int MaxSpawnAttempts
        {
            get => _maxSpawnAttempts;
            set => _maxSpawnAttempts = Mathf.Max(1, value);
        }

        public float SpawnHeight
        {
            get => _spawnHeight;
            set => _spawnHeight = value;
        }

        private void Awake()
        {
            ResolveReferences();
        }

        public void ResolveReferences()
        {
            if (_food == null)
            {
                _food = FindAnyObjectByType<Food>();
            }

            if (_snakeController == null)
            {
                _snakeController = FindAnyObjectByType<SnakeController>();
            }

            if (_arenaBoundary == null)
            {
                _arenaBoundary = FindAnyObjectByType<ArenaBoundary>();
            }
        }

        private void OnEnable()
        {
            if (_food != null)
            {
                _food.OnCollected -= HandleFoodCollected;
                _food.OnCollected += HandleFoodCollected;
            }
        }

        private void OnDisable()
        {
            if (_food != null)
            {
                _food.OnCollected -= HandleFoodCollected;
            }
        }

        private void Start()
        {
            // Initial safe random spawn
            RespawnFood();
        }

        /// <summary>
        /// Handles Food.OnCollected event by repositioning the food to a new safe random position.
        /// </summary>
        private void HandleFoodCollected()
        {
            RespawnFood();
        }

        /// <summary>
        /// Public API to reposition the existing food instance to a safe random location in the arena.
        /// </summary>
        public void RespawnFood()
        {
            ResolveReferences();

            if (_food == null)
            {
                Debug.LogWarning("[FoodSpawner] Food reference is missing, cannot respawn.");
                return;
            }

            Vector3 newPosition = GetSafeRandomPosition();
            _food.SetPosition(newPosition);
            _food.ResetFood();
        }

        /// <summary>
        /// Calculates a safe random position within the arena bounds, checking against snake head and body segments.
        /// </summary>
        public Vector3 GetSafeRandomPosition()
        {
            float minX = -45f;
            float maxX = 45f;
            float minZ = -45f;
            float maxZ = 45f;

            if (_arenaBoundary != null)
            {
                minX = _arenaBoundary.EffectiveMinX + _spawnMargin;
                maxX = _arenaBoundary.EffectiveMaxX - _spawnMargin;
                minZ = _arenaBoundary.EffectiveMinZ + _spawnMargin;
                maxZ = _arenaBoundary.EffectiveMaxZ - _spawnMargin;

                // Guard against inverted bounds if margin is too large
                if (minX > maxX) { float mid = (minX + maxX) * 0.5f; minX = mid - 1f; maxX = mid + 1f; }
                if (minZ > maxZ) { float mid = (minZ + maxZ) * 0.5f; minZ = mid - 1f; maxZ = mid + 1f; }
            }

            Vector3 candidate = new Vector3(0f, _spawnHeight, 0f);
            bool foundSafe = false;

            for (int attempt = 0; attempt < _maxSpawnAttempts; attempt++)
            {
                float randX = Random.Range(minX, maxX);
                float randZ = Random.Range(minZ, maxZ);
                candidate = new Vector3(randX, _spawnHeight, randZ);

                if (IsSafeSpawnPosition(candidate))
                {
                    foundSafe = true;
                    break;
                }
            }

            if (!foundSafe)
            {
                Debug.LogWarning("[FoodSpawner] Could not find fully safe spawn position after max attempts, using best fallback candidate.");
            }

            return candidate;
        }

        /// <summary>
        /// Validates whether candidate world position is safe from snake head and body segments.
        /// </summary>
        public bool IsSafeSpawnPosition(Vector3 position)
        {
            if (_snakeController == null)
            {
                return true;
            }

            float sqrMinDist = _minimumSnakeDistance * _minimumSnakeDistance;

            // Check Snake Head
            if (_snakeController.Head != null)
            {
                Vector3 headPos = _snakeController.Head.transform.position;
                float dx = position.x - headPos.x;
                float dz = position.z - headPos.z;
                if (dx * dx + dz * dz < sqrMinDist)
                {
                    return false;
                }
            }

            // Check Body Segments
            var segments = _snakeController.Segments;
            if (segments != null)
            {
                for (int i = 0; i < segments.Count; i++)
                {
                    var seg = segments[i];
                    if (seg == null) continue;

                    Vector3 segPos = seg.transform.position;
                    float dx = position.x - segPos.x;
                    float dz = position.z - segPos.z;
                    if (dx * dx + dz * dz < sqrMinDist)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
