using UnityEngine.Localization.Settings;

namespace FoxHill.Core.Settings
{
    public class LanguageManager
    {
        public enum LanguageType
        {
            Korean,
            English
        }

        public LanguageManager(LocalizationSettings settings)
        {
            _settings = settings;
            ChangeLanguage(LanguageType.English);
        }

        public LanguageType CurrentLanguage { get; private set; }

        private const string LANGUAGE_KOREAN = "ko-KR";
        private const string LANGUAGE_ENGLISH = "en-US";

        private LocalizationSettings _settings;

        public void ChangeLanguage(LanguageType newLanguage)
        {
            CurrentLanguage = newLanguage;

            switch (newLanguage)
            {
                case LanguageType.Korean:
                    {
                        _settings.SetSelectedLocale(_settings.GetAvailableLocales().GetLocale(LANGUAGE_KOREAN));
                    }
                    break;
                case LanguageType.English:
                    {
                        _settings.SetSelectedLocale(_settings.GetAvailableLocales().GetLocale(LANGUAGE_ENGLISH));
                    }
                    break;
            }

        }
    }
}