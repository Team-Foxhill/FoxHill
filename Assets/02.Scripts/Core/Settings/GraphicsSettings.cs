using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FoxHill.Core.Settings
{
    public class GraphicsSettings : SettingsBase
    {
        public GraphicsSettings(Button button1600, Button button1920)
        {
            _currentIndex = 0;
            _button1600 = new ResolutionButton(button1600);
            _button1920 = new ResolutionButton(button1920);

            _buttonDictionary = new Dictionary<int, ResolutionButton>(2);
            _buttonDictionary.Add(0, _button1600);
            _buttonDictionary.Add(1, _button1920);

            _buttonDictionary[_currentIndex].OnHoverEnter();
        }

        private const int BUTTON_COUNT = 2; // 현재 관리하고 있는 버튼의 개수

        private int _currentIndex = 0;

        private class ResolutionButton
        {
            private readonly Color COLOR_SELECTED = new Color(140f / 255f, 140f / 255f, 140f / 255f, 1f);

            private Button _button;
            private Image _image;

            public ResolutionButton(Button button)
            {
                _button = button;
                _image = _button.GetComponent<Image>();
            }

            public void OnHoverEnter()
            {
                _image.color = COLOR_SELECTED;
            }

            public void OnHoverExit()
            {
                _image.color = Color.white;
            }
        }

        private ResolutionButton _button1600;
        private ResolutionButton _button1920;

        private Dictionary<int, ResolutionButton> _buttonDictionary;

        public void SwitchSelection(bool isLeftward)
        {
            _buttonDictionary[_currentIndex].OnHoverExit();

            if (isLeftward == true && _currentIndex > 0)
            {
                _currentIndex--;
            }

            else if (isLeftward == false && _currentIndex < BUTTON_COUNT - 1)
            {
                _currentIndex++;
            }
            _buttonDictionary[_currentIndex].OnHoverEnter();
        }

        public void SelectOption()
        {
            if (_currentIndex == 0)
            {
                SetScreenResolution(1600, 900);
            }
            else if (_currentIndex == 1)
            {
                SetScreenResolution(1920, 1080);
            }
            else
            {
                Debug.LogError($"Wrong index {_currentIndex} in GraphicsSetting");
            }
        }

        private void SetScreenResolution(int width, int height)
        {
            Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
        }

        public override void OnExit()
        {
            _buttonDictionary[_currentIndex].OnHoverExit();
            _currentIndex = 0;
            _buttonDictionary[_currentIndex].OnHoverEnter();
        }
    }
}
