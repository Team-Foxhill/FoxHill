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
        }

        private const float VOLUME_CONTROL_AMOUNT = 0.1f;

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
