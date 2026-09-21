using System.Collections;
using UnityEngine;

namespace SnakeGame3D.FoodSystem
{
    /// <summary>
    /// Lightweight visual effect controller for the 3D food item.
    /// Handles continuous idle rotation, gentle vertical floating/bobbing, and smooth collection shrink animation.
    /// Preserves base world position set by FoodSpawner.
    /// </summary>
    [RequireComponent(typeof(Food))]
    public class FoodVisualEffect : MonoBehaviour
    {
        [Header("Idle Animation Configuration")]
        [Tooltip("Rotation speed in degrees per second")]
        [SerializeField] private float _rotationSpeed = 90.0f;

        [Tooltip("Amplitude of the vertical bobbing movement")]
        [SerializeField] private float _bobAmplitude = 0.08f;

        [Tooltip("Frequency of the vertical bobbing movement in cycles per second")]
        [SerializeField] private float _bobFrequency = 2.0f;

        [Header("Collection Animation Configuration")]
        [Tooltip("Duration of the shrink-and-pop collection effect in seconds")]
        [SerializeField] private float _collectionEffectDuration = 0.15f;

        private Food _food;
        private Vector3 _initialScale;
        private Coroutine _collectionAnimCoroutine;
        private float _timeAccumulator;

        public float RotationSpeed { get => _rotationSpeed; set => _rotationSpeed = value; }
        public float BobAmplitude { get => _bobAmplitude; set => _bobAmplitude = value; }
        public float BobFrequency { get => _bobFrequency; set => _bobFrequency = value; }
        public float CollectionEffectDuration { get => _collectionEffectDuration; set => _collectionEffectDuration = Mathf.Max(0.01f, value); }

        private void Awake()
        {
            _food = GetComponent<Food>();
            _initialScale = transform.localScale;
        }

        private void OnEnable()
        {
            if (_food != null)
            {
                _food.OnCollected -= HandleFoodCollected;
                _food.OnCollected += HandleFoodCollected;
            }
            ResetVisualState();
        }

        private void OnDisable()
        {
            if (_food != null)
            {
                _food.OnCollected -= HandleFoodCollected;
            }
            if (_collectionAnimCoroutine != null)
            {
                StopCoroutine(_collectionAnimCoroutine);
                _collectionAnimCoroutine = null;
            }
        }

        private void Update()
        {
            if (_food != null && _food.IsCollected)
            {
                return;
            }

            // Continuous rotation
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.World);

            // Subtle vertical floating/bobbing
            _timeAccumulator += Time.deltaTime;
            float bobOffset = Mathf.Sin(_timeAccumulator * _bobFrequency * Mathf.PI * 2f) * _bobAmplitude;

            Vector3 currentPos = transform.position;
            float baseY = _food != null ? _food.GroundYOffset : 0.4f;
            currentPos.y = baseY + bobOffset;
            transform.position = currentPos;
        }

        private void HandleFoodCollected()
        {
            if (_collectionAnimCoroutine != null)
            {
                StopCoroutine(_collectionAnimCoroutine);
            }
            _collectionAnimCoroutine = StartCoroutine(PlayCollectionAnimation());
        }

        private IEnumerator PlayCollectionAnimation()
        {
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;

            while (elapsed < _collectionEffectDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / _collectionEffectDuration);
                // Ease out scale to 0
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                yield return null;
            }

            transform.localScale = Vector3.zero;
            _collectionAnimCoroutine = null;
        }

        /// <summary>
        /// Resets the visual scale and animation state cleanly upon respawn/restart.
        /// </summary>
        public void ResetVisualState()
        {
            if (_collectionAnimCoroutine != null)
            {
                StopCoroutine(_collectionAnimCoroutine);
                _collectionAnimCoroutine = null;
            }

            transform.localScale = _initialScale;
            _timeAccumulator = 0f;
        }
    }
}
