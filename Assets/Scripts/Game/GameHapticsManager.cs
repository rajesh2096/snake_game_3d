using UnityEngine;

namespace SnakeGame3D.Game
{
    /// <summary>
    /// Safe mobile haptics / vibration abstraction.
    /// Safely handles calls without throwing exceptions on non-supported platforms.
    /// </summary>
    public static class GameHapticsManager
    {
        public static void Vibrate()
        {
            if (GameSettingsManager.Instance != null && !GameSettingsManager.Instance.VibrationEnabled)
            {
                return;
            }

            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Handheld.Vibrate();
#endif
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[GameHapticsManager] Vibrate failed: {ex.Message}");
            }
        }
    }
}
