using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoxHill.Core.Settings
{
    public class GraphicsSettings : SettingsBase
    {
        public GraphicsSettings(Image screenModeSelector, Image resolutionSelector)
        {
            _currentIndex = -1;
            _screenModeSelector = new SettingSelector(screenModeSelector, new List<SettingSelection>
            {
                new SettingSelection("전체 화면", () => { _screenMode = FullScreenMode.ExclusiveFullScreen; }),
                new SettingSelection("창 화면", () => { _screenMode = FullScreenMode. Windowed; }),
            });

            _resolutionSelector = new SettingSelector(resolutionSelector, new List<SettingSelection>
            {
                new SettingSelection("1920x1080", () => { _width = 1920; _height = 1080; }),
                new SettingSelection("1600x900", () => { _width = 1600; _height = 900; }),
            });

            _buttonDictionary = new Dictionary<int, SettingSelector>(2)
            {
                { 0, _screenModeSelector },
                { 1, _resolutionSelector }
            };
        }

        private const int BUTTON_COUNT = 2; // 현재 관리하고 있는 버튼의 개수

        private int _currentIndex;

        private SettingSelector _screenModeSelector;
        private SettingSelector _resolutionSelector;

        private Dictionary<int, SettingSelector> _buttonDictionary;

        private FullScreenMode _screenMode = FullScreenMode.ExclusiveFullScreen;
        private int _width = 1920;
        private int _height = 1080;


        public void SwitchSelection(bool isLeftward)
        {
            if (_currentIndex == -1) // 초기 상태에서 입력을 받으면
            {
                _currentIndex = 0;
                _buttonDictionary[_currentIndex].OnHoverEnter();
                return;
            }

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

        /// <summary>
        /// 선택된 Graphics Option들을 적용
        /// </summary>
        public void SelectOption()
        {
            for (int i = 0; i < BUTTON_COUNT; i++)
            {
                _buttonDictionary[i].OnSelect();
            }

            SetScreenResolution();
        }

        public void SwitchOption(bool isUpward)
        {
            if(_currentIndex == -1) // 초기 상태
            {
                return;
            }

            if (isUpward == true)
            {
                _buttonDictionary[_currentIndex].OnSwipeUp();
            }
            else
            {
                _buttonDictionary[_currentIndex].OnSwipeDown();
            }
        }

        private void SetScreenResolution()
        {
            Screen.SetResolution(_width, _height, _screenMode);
        }

        public override void OnExitSettingSelection()
        {
            _buttonDictionary[_currentIndex].OnHoverExit();
            _currentIndex = -1;
        }

        public override void OnSettingClosed()
        {
            if(_currentIndex == -1) // 초기 상태
            {
                return;
            }

            _buttonDictionary[_currentIndex].OnHoverExit();
            _currentIndex = -1;
        }
    }
}
