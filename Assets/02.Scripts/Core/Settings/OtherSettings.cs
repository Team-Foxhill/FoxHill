using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace FoxHill.Core.Settings
{
    public class OtherSettings : SettingsBase
    {
        public OtherSettings(Image languageSelector)
        {
            _languageSelector = new SettingSelector(languageSelector, new List<SettingSelection>
            {
                new SettingSelection(
                    languageSelector.transform.Find("Text (TMP)_Korean").GetComponent<TMP_Text>(),
                    () => {GameManager.Instance.Language.ChangeLanguage(LanguageManager.LanguageType.Korean); }),
                new SettingSelection(
                    languageSelector.transform.Find("Text (TMP)_English").GetComponent<TMP_Text>(),
                    () => {GameManager.Instance.Language.ChangeLanguage(LanguageManager.LanguageType.English); }),
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
            _isActivated = false;
            _languageSelector.OnHoverExit();
        }

        public override void OnSettingClosed()
        {
            _isActivated = false;
            _languageSelector.OnHoverExit();
        }
    }
}
