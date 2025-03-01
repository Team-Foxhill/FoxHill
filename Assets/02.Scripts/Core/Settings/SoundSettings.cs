using System;
using UnityEngine;
using UnityEngine.UI;

namespace FoxHill.Core.Settings
{
    public class SoundSettings : SettingsBase
    {
        public float Volume => _volumeSlider.value;
        public SoundSettings(Slider volumeSlider)
        {
            _volumeSlider = volumeSlider;
            _volumeSlider.value = 1f;
        }

        public Action<float> OnVolumeChanged;
        private const float VOLUME_CONTROL_AMOUNT = 0.05f;

        private Slider _volumeSlider;

        public void SetVolumeSlider(bool isTurnDown)
        {
            if (isTurnDown == true)
            {
                _volumeSlider.value
                    = (_volumeSlider.value - VOLUME_CONTROL_AMOUNT >= 0f)
                    ? _volumeSlider.value - VOLUME_CONTROL_AMOUNT
                    : 0f;
            }
            else
            {
                _volumeSlider.value
                    = (_volumeSlider.value + VOLUME_CONTROL_AMOUNT <= 1f)
                    ? _volumeSlider.value + VOLUME_CONTROL_AMOUNT
                    : 1f;
            }
            OnVolumeChanged.Invoke(_volumeSlider.value);
        }

        public override void OnExitSettingSelection()
        {
            return;
        }

        public override void OnSettingClosed()
        {
            return;
        }

    }
}
