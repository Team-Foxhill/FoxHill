using TMPro;

namespace FoxHill.Core.Settings
{
    public class OtherSettings : SettingsBase
    {
        public OtherSettings(TMP_Dropdown dropdown) 
        { 
            _dropdown = dropdown;
        }

        public bool IsDropdownExpanded { get; private set; } = false;

        private TMP_Dropdown _dropdown;

        public void ToggleDropdown(bool toggle)
        {
            if(toggle == true)
            {
                _dropdown.Show();
                IsDropdownExpanded = true;
            }
            else
            {
                _dropdown.Hide();
                IsDropdownExpanded = false;
            }
        }

        public override void OnExit()
        {
            ToggleDropdown(false);
        }
    }
}
