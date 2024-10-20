using FoxHill.Core.Settings;
using System.Collections.Generic;
namespace FoxHill.Audio
{
    /// <summary>
    /// 게임 내 모든 사운드에 볼륨을 적용해주는 클래스.
    /// </summary>
    public static class SoundVolumeManager
    {
        private static HashSet<IVolumeAdjustable> playerbles = new HashSet<IVolumeAdjustable>(1024);
        private static bool _isPaused = false;

        public static void LinkSoundSetting(SoundSettings soundSettings)
        {
            soundSettings.OnVolumeChanged += OnVolumeChanged;
        }

        public static void UnLinkSoundSetting(SoundSettings soundSettings)
        {
            soundSettings.OnVolumeChanged -= OnVolumeChanged;
        }

        public static void Register(IVolumeAdjustable playerble)
        {
            playerbles.Add(playerble);
        }

        public static void Unregister(IVolumeAdjustable playerble)
        {
            playerbles.Remove(playerble);
        }

        private static void OnVolumeChanged(float volume)
        {
            foreach (IVolumeAdjustable playerble in playerbles)
            {
                playerble.OnVolumeChanged(volume);
            }
        }
    }
}
