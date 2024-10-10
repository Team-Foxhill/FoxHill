using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoxHill.Core.Settings
{
    public class SettingSelector
    {
        private readonly Color COLOR_SELECTED = new Color(140f / 255f, 140f / 255f, 140f / 255f, 1f);

        private int _currentIndex;
        private int _length;
        private Image _image;
        private List<SettingSelection> _selections = new List<SettingSelection>(3);
        private GameObject _arrows;

        public SettingSelector(Image image, List<SettingSelection> selections)
        {
            _currentIndex = 0;
            _image = image;
            _selections = selections;
            _length = selections.Count;
            _arrows = image.transform.Find("Arrows").gameObject;

            Initialize();
            OnHoverExit();
        }

        public void OnHoverEnter()
        {
            _image.color = COLOR_SELECTED;
            _arrows.SetActive(true);
        }

        public void OnHoverExit()
        {
            _image.color = Color.white;
            _arrows.SetActive(false);
        }

        public void OnSwipeUp()
        {
            _selections[_currentIndex].Toggle(false);
            _currentIndex = (_currentIndex - 1 >= 0) ? _currentIndex - 1 : _length - 1;
            _selections[_currentIndex].Toggle(true);
        }

        public void OnSwipeDown()
        {
            _selections[_currentIndex].Toggle(false);
            _currentIndex = (_currentIndex + 1 < _length) ? _currentIndex + 1 : 0;
            _selections[_currentIndex].Toggle(true);
        }

        public void OnSelect()
        {
            _selections[_currentIndex]?.Select();
        }

        private void Initialize()
        {
            _selections[0].Toggle(true);

            int selectionCount = _selections.Count;
            for (int i = 1; i < selectionCount; i++)
            {
                _selections[i].Toggle(false);
            }
        }
    }
}