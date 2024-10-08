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
        private TMP_Text _text;
        private List<SettingSelection> _selections = new List<SettingSelection>(3);
        private GameObject _arrows;

        public SettingSelector(Image image, List<SettingSelection> selections)
        {
            _currentIndex = 0;
            _image = image;
            _text = image.transform.GetChild(0).GetComponent<TMP_Text>();
            _selections = selections;
            _length = selections.Count;
            _arrows = image.transform.Find("Arrows").gameObject;

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
            _currentIndex = (_currentIndex - 1 >= 0) ? _currentIndex - 1 : _length - 1;
            _text.text = _selections[_currentIndex].Text;
        }

        public void OnSwipeDown()
        {
            _currentIndex = (_currentIndex + 1 < _length) ? _currentIndex + 1 : 0;
            _text.text = _selections[_currentIndex].Text;
        }

        public void OnSelect()
        {
            _selections[_currentIndex]?.Select();
        }
    }
}