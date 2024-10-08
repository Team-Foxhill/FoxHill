namespace FoxHill.Core.Settings
{
    public class LanguageManager
    {
        public enum LanguageType
        {
            Korean,
            English
        }

        public LanguageType CurrentLanguage { get; private set; } 
        
        public void ChangeLanguage(LanguageType newLanguage)
        {
            CurrentLanguage = newLanguage;
        }
    }
}