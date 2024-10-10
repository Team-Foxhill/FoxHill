using System;
using TMPro;

namespace FoxHill.Core.Settings
{
    public class SettingSelection
    {
        public SettingSelection(TMP_Text selectionText, Action action)
        {
            _selectionText = selectionText;
            _action = action;
        }

        public TMP_Text Text => _selectionText;

        private TMP_Text _selectionText;
        private Action _action;


        public void Select()
        {
            _action?.Invoke();
        }

        public void Toggle(bool toggle)
        {
            _selectionText.enabled = toggle;
        }
    }
}