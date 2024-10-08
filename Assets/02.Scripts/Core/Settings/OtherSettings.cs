using System.Collections.Generic;
using UnityEngine.UI;

namespace FoxHill.Core.Settings
{
    public class OtherSettings : SettingsBase
    {
        public OtherSettings(Image languageSelector)
        {
            _languageSelector = new SettingSelector(languageSelector, new List<SettingSelection>
            {
                new SettingSelection("한국어", () => {GameManager.Instance.Language.ChangeLanguage(LanguageManager.LanguageType.Korean); }),
                new SettingSelection("English", () => {GameManager.Instance.Language.ChangeLanguage(LanguageManager.LanguageType.English); }),
            });
        }

        private bool _isActivated = false;
        private SettingSelector _languageSelector;

        public void SwitchSelection(bool isLeftward)
        {
            _languageSelector.OnHoverEnter();
            if(_isActivated == false)
            {
                _isActivated = true;
            }
        }

        public void SwitchOption(bool isUpward)
        {
            if (_isActivated == false)
            {
                return;
            }

            if (isUpward == true)
            {
                _languageSelector.OnSwipeUp();
            }
            else
            {
                _languageSelector.OnSwipeDown();
            }
        }

        public void SelectOption()
        {
            _languageSelector.OnSelect();
        }
        public override void OnExitSettingSelection()
        {
            _languageSelector.OnHoverExit();
        }

        public override void OnSettingClosed()
        {
            _languageSelector.OnHoverExit();
        }
    }
}
