using FoxHill.Core.Settings;
using FoxHill.Player.Skill;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;
namespace FoxHill.Audio
{
    /// <summary>
    /// 게임 내 모든 사운드에 볼륨을 적용해주는 클래스.
    /// </summary>
    public static class SoundVolumeManager
    {
        private static HashSet<IVolumeAdjustable> playerbles = new HashSet<IVolumeAdjustable>(1024);
        private static PlayerSkillController _playerSkillController;
        public static float VolumeValue => _lastValue;
        private static float _lastValue;
        private static bool _isVolumeChangedOnceOrMore;

        public static void SetInitialVolume(Slider slider)
        {
            _lastValue = slider.value;
            OnVolumeChanged(slider.value);
        }


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
            if (_isVolumeChangedOnceOrMore == false)
            {
                return;
            }
            playerble.OnVolumeChanged(_lastValue);
        }

        public static void Unregister(IVolumeAdjustable playerble)
        {
            playerbles.Remove(playerble);
        }

        private static void OnVolumeChanged(float volume)
        {
            if (_isVolumeChangedOnceOrMore == false)
            {
                _isVolumeChangedOnceOrMore = true;
            }
            _lastValue = volume;

            foreach (IVolumeAdjustable playerble in playerbles)
            {
                playerble.OnVolumeChanged(volume);
            }
        }
    }
}
