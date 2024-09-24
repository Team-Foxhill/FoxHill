using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FoxHill.Core.Settings
{
    [RequireComponent(typeof(Canvas))]
    public class GameSettingsManager : MonoBehaviour, MenuInputAction.ITitleSceneSettingsActions
    {
        public MenuInputAction InputAction
        {
            get => _inputAction;
            set
            {
                if (_inputAction == null)
                {
                    _inputAction = value;
                }
            }
        }

        public bool IsEnabled => _settingsCanvas.enabled;

        private enum SettingOptions
        {
            Sound,
            Graphics,
            Other
        }

        [System.Serializable]
        private class SettingType
        {
            public TMP_Text Text;
            public SettingsBase Setting { get; private set; }

            public void Initialize(SettingsBase settings)
            {
                Setting = settings;
            }
        }

        [SerializeField] private SettingType _soundSetting;
        [SerializeField] private SettingType _graphicsSetting;
        [SerializeField] private SettingType _otherSetting;

        private SettingOptions _currentOption = 0;
        private Dictionary<SettingOptions, SettingType> _optionDictionary = new Dictionary<SettingOptions, SettingType>(3);

        private MenuInputAction _inputAction = null; // 프로퍼티를 통해 외부 클래스에게서 공유받아 ActionMap을 스위칭하여 사용.
        private Canvas _settingsCanvas;

        /// <summary>
        /// 커스터마이즈할 설정을 선택했는지를 관리하는 변수
        /// </summary>
        private bool _isSettingSelected = false;


        // 캔버스 내에 배치된 UI 요소들
        #region UI Elements 
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private Button _resolutionButton1600;
        [SerializeField] private Button _resolutionButton1920;
        [SerializeField] private TMP_Dropdown _languageDropdown;
        #endregion


        private void Awake()
        {
            _settingsCanvas = GetComponent<Canvas>();

            _soundSetting.Initialize(new SoundSettings(_volumeSlider));
            _graphicsSetting.Initialize(new GraphicsSettings(_resolutionButton1600, _resolutionButton1920));
            _otherSetting.Initialize(new OtherSettings(_languageDropdown));

            _optionDictionary.Add(SettingOptions.Sound, _soundSetting);
            _optionDictionary.Add(SettingOptions.Graphics, _graphicsSetting);
            _optionDictionary.Add(SettingOptions.Other, _otherSetting);
        }

        private void Start()
        {
            ToggleUI(false);
        }

        public void ToggleUI(bool toggle)
        {
            _settingsCanvas.enabled = toggle;

            if (toggle == false)
            {
                _inputAction.TitleSceneSettings.Disable();
                _inputAction.TitleScene.Enable();
                OnHoverSettingExit();

                _soundSetting.Setting.OnExit();
                _graphicsSetting.Setting.OnExit();
                _otherSetting.Setting.OnExit();
            }
            else
            {
                _inputAction.TitleScene.Disable();
                _inputAction.TitleSceneSettings.Enable();
                _currentOption = 0;
                _isSettingSelected = false;
                OnHoverSettingEnter();
            }
        }

        public void OnSwitchMenu(InputAction.CallbackContext context) // Arrow
        {
            if (context.started == true && IsEnabled == true)
            {
                var input = context.ReadValue<Vector2>();

                // 아직 커스텀할 설정이 고정되지 않은 상태라면 설정 간에 이동 수행
                if (_isSettingSelected == false)
                {
                    OnHoverSettingExit();
                    if (input == Vector2.up)
                    {
                        _currentOption = (_currentOption == 0) ? _currentOption : _currentOption - 1;
                    }
                    else if (input == Vector2.down)
                    {
                        _currentOption = (_currentOption == SettingOptions.Other) ? _currentOption : _currentOption + 1;
                    }
                    OnHoverSettingEnter();
                }
                else
                {
                    // 커스텀할 설정이 고정된 상태라면 해당 설정 내 옵션 간에 이동 수행
                    switch (_currentOption)
                    {
                        case SettingOptions.Sound:
                            {
                                if (input == Vector2.left)
                                {
                                    (_soundSetting.Setting as SoundSettings).SetVolumeSlider(true);
                                }
                                else if (input == Vector2.right)
                                {
                                    (_soundSetting.Setting as SoundSettings).SetVolumeSlider(false);
                                }
                            }
                            break;
                        case SettingOptions.Graphics:
                            {
                                if (input == Vector2.left)
                                {
                                    (_graphicsSetting.Setting as GraphicsSettings).SwitchSelection(true);
                                }
                                else if (input == Vector2.right)
                                {
                                    (_graphicsSetting.Setting as GraphicsSettings).SwitchSelection(false);
                                }
                            }
                            break;
                        case SettingOptions.Other:
                            {
                            }
                            break;
                    }

                }
            }
        }

        public void OnSelect(InputAction.CallbackContext context) // Z, Enter
        {
            if (context.started == true && IsEnabled == true)
            {
                // 아직 커스텀할 설정이 고정되지 않은 상태라면 Select키로 고정
                if (_isSettingSelected == false)
                {
                    _isSettingSelected = true;
                    OnSelectSetting();
                    return;
                }

                switch (_currentOption)
                {
                    case SettingOptions.Sound:
                        {
                        }
                        break;
                    case SettingOptions.Graphics:
                        {
                            (_graphicsSetting.Setting as GraphicsSettings).SelectOption();
                        }
                        break;
                    case SettingOptions.Other:
                        {
                            (_otherSetting.Setting as OtherSettings).ToggleDropdown(true);
                        }
                        break;
                }
            }
        }

        public void OnDeselect(InputAction.CallbackContext context) // X, Esc
        {
            if (context.started == true && IsEnabled == true)
            {
                // 아직 커스텀할 설정이 고정되지 않은 상태라면 캔버스를 끕니다.
                if (_isSettingSelected == false)
                {
                    ToggleUI(false);
                    return;
                }
                 
                switch (_currentOption)
                    {
                        case SettingOptions.Sound:
                            {
                                _isSettingSelected = false;
                                OnHoverSettingEnter();
                            }
                            break;
                        case SettingOptions.Graphics:
                            {
                                _isSettingSelected = false;
                                OnHoverSettingEnter();
                            }
                            break;
                        case SettingOptions.Other:
                            {
                                var otherSetting = _otherSetting.Setting as OtherSettings;

                                if (otherSetting.IsDropdownExpanded == true)
                                {
                                    otherSetting.ToggleDropdown(false);
                                }
                                else
                                {
                                    _isSettingSelected = false;
                                    OnHoverSettingEnter();
                                }
                            }
                            break;
                    }
                
            }
        }

        private void OnHoverSettingEnter()
        {
            _optionDictionary[_currentOption].Text.color = Color.gray;
        }

        private void OnSelectSetting()
        {
            _optionDictionary[_currentOption].Text.color = Color.black;
        }

        private void OnHoverSettingExit()
        {
            _optionDictionary[_currentOption].Text.color = Color.white;
        }

    }
}
