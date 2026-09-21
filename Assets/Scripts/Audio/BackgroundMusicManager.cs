using UnityEngine;
using SnakeGame3D.Game;

namespace SnakeGame3D.Audio
{
    /// <summary>
    /// Coordinates looping background music.
    /// Music starts on GameStartManager.OnGameStarted, pauses during Pause and resumes on Resume,
    /// and cleanly stops on Game Over and Restart.
    /// Handles missing audio clips safely without exceptions.
    /// Supports dynamic music enabling and volume control via GameSettingsManager.
    /// </summary>
    public class BackgroundMusicManager : MonoBehaviour
    {
        [Header("Audio Source")]
        [Tooltip("AudioSource component used for playing background music")]
        [SerializeField] private AudioSource _musicSource;

        [Header("Music Configuration")]
        [Tooltip("Background music audio track")]
        [SerializeField] private AudioClip _backgroundMusicClip;

        [Range(0f, 1f)]
        [SerializeField] private float _musicVolume = 0.6f;

        [SerializeField] private bool _musicEnabled = true;
        [SerializeField] private bool _loop = true;

        [Header("System References (Optional manual binding)")]
        [SerializeField] private GameStartManager _startManager;
        [SerializeField] private GamePauseManager _pauseManager;
        [SerializeField] private GameOverManager _gameOverManager;

        public AudioSource MusicSource { get => _musicSource; set => _musicSource = value; }
        public AudioClip BackgroundMusicClip { get => _backgroundMusicClip; set => _backgroundMusicClip = value; }

        public bool MusicEnabled
        {
            get => _musicEnabled;
            set
            {
                _musicEnabled = value;
                if (!_musicEnabled)
                {
                    StopMusic();
                }
                else if (_startManager != null && _startManager.IsGameStarted && (_pauseManager == null || !_pauseManager.IsPaused))
                {
                    PlayMusic();
                }
            }
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Mathf.Clamp01(value);
                if (_musicSource != null)
                {
                    _musicSource.volume = _musicVolume;
                }
            }
        }

        public bool Loop
        {
            get => _loop;
            set
            {
                _loop = value;
                if (_musicSource != null)
                {
                    _musicSource.loop = _loop;
                }
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

        private void Awake()
        {
            EnsureAudioSource();
            ResolveReferences();
            LoadInitialSettings();
        }

        private void LoadInitialSettings()
        {
            if (GameSettingsManager.Instance != null)
            {
                _musicEnabled = GameSettingsManager.Instance.MusicEnabled;
                _musicVolume = GameSettingsManager.Instance.MusicVolume;
            }
            else
            {
                _musicEnabled = PlayerPrefs.GetInt(GameSettingsManager.KeyMusicEnabled, 1) == 1;
                _musicVolume = PlayerPrefs.GetFloat(GameSettingsManager.KeyMusicVolume, 0.6f);
            }
        }

        public void SetMusicEnabled(bool enabled)
        {
            MusicEnabled = enabled;
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;
        }

        public void EnsureAudioSource()
        {
            if (_musicSource == null)
            {
                _musicSource = GetComponent<AudioSource>();
                if (_musicSource == null)
                {
                    _musicSource = gameObject.AddComponent<AudioSource>();
                }
            }

            _musicSource.playOnAwake = false;
            _musicSource.loop = _loop;
            _musicSource.volume = _musicVolume;
        }

        public void ResolveReferences()
        {
            if (_startManager == null) _startManager = FindAnyObjectByType<GameStartManager>();
            if (_pauseManager == null) _pauseManager = FindAnyObjectByType<GamePauseManager>();
            if (_gameOverManager == null) _gameOverManager = FindAnyObjectByType<GameOverManager>();
        }

        private void OnEnable()
        {
            ResolveReferences();

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

            if (_gameOverManager != null)
            {
                _gameOverManager.OnGameOver -= HandleGameOver;
                _gameOverManager.OnGameOver += HandleGameOver;
            }
        }

        private void OnDisable()
        {
            if (_startManager != null) _startManager.OnGameStarted -= HandleGameStarted;
            if (_pauseManager != null) _pauseManager.OnPauseStateChanged -= HandlePauseStateChanged;
            if (_gameOverManager != null) _gameOverManager.OnGameOver -= HandleGameOver;
        }

        /// <summary>
        /// Starts playing background music from the beginning.
        /// </summary>
        public void PlayMusic()
        {
            if (!_musicEnabled || _backgroundMusicClip == null)
            {
                return;
            }

            EnsureAudioSource();
            _musicSource.clip = _backgroundMusicClip;
            _musicSource.loop = _loop;
            _musicSource.volume = _musicVolume;
            _musicSource.Play();
            Debug.Log("[BackgroundMusicManager] Background music playing.");
        }

        /// <summary>
        /// Pauses background music playback.
        /// </summary>
        public void PauseMusic()
        {
            if (_musicSource != null && _musicSource.isPlaying)
            {
                _musicSource.Pause();
                Debug.Log("[BackgroundMusicManager] Background music paused.");
            }
        }

        /// <summary>
        /// Resumes background music from current paused position.
        /// </summary>
        public void ResumeMusic()
        {
            if (!_musicEnabled || _backgroundMusicClip == null)
            {
                return;
            }

            if (_musicSource != null)
            {
                _musicSource.UnPause();
                if (!_musicSource.isPlaying)
                {
                    _musicSource.Play();
                }
                Debug.Log("[BackgroundMusicManager] Background music resumed.");
            }
        }

        /// <summary>
        /// Stops background music completely.
        /// </summary>
        public void StopMusic()
        {
            if (_musicSource != null)
            {
                _musicSource.Stop();
                Debug.Log("[BackgroundMusicManager] Background music stopped.");
            }
        }

        private void HandleGameStarted()
        {
            PlayMusic();
        }

        private void HandlePauseStateChanged(bool isPaused)
        {
            if (isPaused)
            {
                PauseMusic();
            }
            else
            {
                ResumeMusic();
            }
        }

        private void HandleGameOver()
        {
            StopMusic();
        }
    }
}
