using System;

namespace FoxHill.Core.Settings
{
    public class SettingSelection
    {
        public SettingSelection(string selectionText, Action action)
        {
            _selectionText = selectionText;
            _action = action;
        }
        public string Text => _selectionText;

        private string _selectionText;
        private Action _action;

        public void Select()
        {
            _action?.Invoke();
        }
    }
}