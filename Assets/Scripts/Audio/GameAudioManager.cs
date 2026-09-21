using UnityEngine;
using SnakeGame3D.FoodSystem;
using SnakeGame3D.Game;

namespace SnakeGame3D.Audio
{
    /// <summary>
    /// Central audio manager for game sound effects.
    /// Subscribes to Food.OnCollected, GameOverManager.OnGameOver, GameStartManager.OnGameStarted,
    /// and GamePauseManager.OnPauseStateChanged. Safely handles missing clips and audio sources without null errors.
    /// </summary>
    public class GameAudioManager : MonoBehaviour
    {
        [Header("Audio Source")]
        [Tooltip("AudioSource used to play SFX (will create/find one if unassigned)")]
        [SerializeField] private AudioSource _sfxSource;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip _foodCollectedClip;
        [SerializeField] private AudioClip _gameOverClip;
        [SerializeField] private AudioClip _gameStartClip;
        [SerializeField] private AudioClip _buttonClickClip;
        [SerializeField] private AudioClip _pauseClip;
        [SerializeField] private AudioClip _resumeClip;

        [Header("Volumes")]
        [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 1f;

        [Header("System References (Optional manual binding)")]
        [SerializeField] private Food _food;
        [SerializeField] private GameOverManager _gameOverManager;
        [SerializeField] private GameStartManager _startManager;
        [SerializeField] private GamePauseManager _pauseManager;

        public AudioSource SfxSource { get => _sfxSource; set => _sfxSource = value; }
        public AudioClip FoodCollectedClip { get => _foodCollectedClip; set => _foodCollectedClip = value; }
        public AudioClip GameOverClip { get => _gameOverClip; set => _gameOverClip = value; }
        public AudioClip GameStartClip { get => _gameStartClip; set => _gameStartClip = value; }
        public AudioClip ButtonClickClip { get => _buttonClickClip; set => _buttonClickClip = value; }
        public AudioClip PauseClip { get => _pauseClip; set => _pauseClip = value; }
        public AudioClip ResumeClip { get => _resumeClip; set => _resumeClip = value; }

        public Food Food
        {
            get => _food;
            set
            {
                if (_food != null) _food.OnCollected -= HandleFoodCollected;
                _food = value;
                if (_food != null && isActiveAndEnabled) _food.OnCollected += HandleFoodCollected;
            }
        }

        public GameOverManager GameOverManager
        {
            get => _gameOverManager;
            set
            {
                if (_gameOverManager != null) _gameOverManager.OnGameOver -= HandleGameOver;
                _gameOverManager = value;
                if (_gameOverManager != null && isActiveAndEnabled) _gameOverManager.OnGameOver += HandleGameOver;
            }
        }

        public GameStartManager StartManager
        {
            get => _startManager;
            set
            {
                if (_startManager != null) _startManager.OnGameStarted -= HandleGameStarted;
                _startManager = value;
                if (_startManager != null && isActiveAndEnabled) _startManager.OnGameStarted += HandleGameStarted;
            }
        }

        public GamePauseManager PauseManager
        {
            get => _pauseManager;
            set
            {
                if (_pauseManager != null) _pauseManager.OnPauseStateChanged -= HandlePauseStateChanged;
                _pauseManager = value;
                if (_pauseManager != null && isActiveAndEnabled) _pauseManager.OnPauseStateChanged += HandlePauseStateChanged;
            }
        }

        private void Awake()
        {
            EnsureAudioSource();
            ResolveReferences();
        }

        public void EnsureAudioSource()
        {
            if (_sfxSource == null)
            {
                _sfxSource = GetComponent<AudioSource>();
                if (_sfxSource == null)
                {
                    _sfxSource = gameObject.AddComponent<AudioSource>();
                }
            }
            _sfxSource.playOnAwake = false;
            _sfxSource.loop = false;
        }

        public void ResolveReferences()
        {
            if (_food == null) _food = FindAnyObjectByType<Food>();
            if (_gameOverManager == null) _gameOverManager = FindAnyObjectByType<GameOverManager>();
            if (_startManager == null) _startManager = FindAnyObjectByType<GameStartManager>();
            if (_pauseManager == null) _pauseManager = FindAnyObjectByType<GamePauseManager>();
        }

        private void OnEnable()
        {
            ResolveReferences();

            if (_food != null)
            {
                _food.OnCollected -= HandleFoodCollected;
                _food.OnCollected += HandleFoodCollected;
            }

            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
                _gameOverManager.OnGameOver += HandleGameOver;
            }

            if (_startManager != null)
            {
                _startManager.OnGameStarted -= HandleGameStarted;
                _startManager.OnGameStarted += HandleGameStarted;
            }

            if (_pauseManager != null)
            {
                _pauseManager.OnPauseStateChanged -= HandlePauseStateChanged;
                _pauseManager.OnPauseStateChanged += HandlePauseStateChanged;
            }
        }

        private void OnDisable()
        {
            if (_food != null) _food.OnCollected -= HandleFoodCollected;
            if (_gameOverManager != null) _gameOverManager.OnGameOver -= HandleGameOver;
            if (_startManager != null) _startManager.OnGameStarted -= HandleGameStarted;
            if (_pauseManager != null) _pauseManager.OnPauseStateChanged -= HandlePauseStateChanged;
        }

        private void HandleFoodCollected() => PlayFoodCollected();
        private void HandleGameOver() => PlayGameOver();
        private void HandleGameStarted() => PlayGameStart();
        private void HandlePauseStateChanged(bool isPaused)
        {
            if (isPaused) PlayPause();
            else PlayResume();
        }

        public void PlayFoodCollected() => PlayClip(_foodCollectedClip);
        public void PlayGameOver() => PlayClip(_gameOverClip);
        public void PlayGameStart() => PlayClip(_gameStartClip);
        public void PlayButtonClick() => PlayClip(_buttonClickClip);
        public void PlayPause() => PlayClip(_pauseClip);
        public void PlayResume() => PlayClip(_resumeClip);

        public void PlayClip(AudioClip clip)
        {
            if (clip == null) return;

            EnsureAudioSource();
            if (_sfxSource != null)
            {
                _sfxSource.PlayOneShot(clip, _sfxVolume);
            }
        }
    }
}
