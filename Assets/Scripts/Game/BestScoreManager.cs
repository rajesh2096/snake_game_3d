using System;
using UnityEngine;

namespace SnakeGame3D.Game
{
    /// <summary>
    /// Tracks, persists, and provides access to the player's Best / High Score.
    /// Uses PlayerPrefs with the key 'SnakeGame_BestScore'.
    /// Updates only when current score exceeds best score.
    /// </summary>
    public class BestScoreManager : MonoBehaviour
    {
        public const string KeyBestScore = "SnakeGame_BestScore";

        [SerializeField] private int _bestScore = 0;

        /// <summary>
        /// Raised whenever the Best Score changes.
        /// </summary>
        public event Action<int> OnBestScoreChanged;

        public int BestScore => _bestScore;

        private void Awake()
        {
            LoadBestScore();
        }

        /// <summary>
        /// Loads the saved best score from PlayerPrefs.
        /// </summary>
        public void LoadBestScore()
        {
            _bestScore = PlayerPrefs.GetInt(KeyBestScore, 0);
            OnBestScoreChanged?.Invoke(_bestScore);
        }

        /// <summary>
        /// Saves the current best score to PlayerPrefs.
        /// </summary>
        public void SaveBestScore()
        {
            PlayerPrefs.SetInt(KeyBestScore, _bestScore);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Evaluates currentScore against BestScore. If higher, updates and saves it.
        /// Returns true if a new high score was set.
        /// </summary>
        public bool TryUpdateBestScore(int currentScore)
        {
            if (currentScore > _bestScore)
            {
                _bestScore = currentScore;
                SaveBestScore();
                OnBestScoreChanged?.Invoke(_bestScore);
                Debug.Log($"[BestScoreManager] New Best Score achieved: {_bestScore}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resets the saved Best Score to 0 (typically requested from Settings).
        /// </summary>
        public void ResetBestScore()
        {
            _bestScore = 0;
            SaveBestScore();
            OnBestScoreChanged?.Invoke(_bestScore);
            Debug.Log("[BestScoreManager] Best Score reset to 0.");
        }
    }
}
