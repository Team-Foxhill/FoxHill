using UnityEngine;
using UnityEngine.InputSystem;

namespace FoxHill.Core.Settings
{
    [RequireComponent (typeof (Canvas))]
    public class GameSettingsManager : MonoBehaviour, MenuInputAction.ITitleSceneActions
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

        private SettingOptions _currentOption = 0;
        private MenuInputAction _inputAction = null; // 프로퍼티를 통해 외부 클래스에게서 공유받아 사용
        private Canvas _settingsCanvas;

        private void Awake()
        {
            _settingsCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            ToggleUI(false);
        }

        public void ToggleUI(bool toggle)
        {
            _settingsCanvas.enabled = toggle;
        }

        public void OnSwitchMenu(InputAction.CallbackContext context) // Arrow
        {
            if (context.started == true && IsEnabled == true)
            {
                var input = context.ReadValue<Vector2>();

                if (input == Vector2.up)
                {
                    _currentOption = (_currentOption == 0) ? _currentOption : _currentOption - 1;
                }
                else if (input == Vector2.down)
                {
                    _currentOption = (_currentOption == SettingOptions.Other) ? _currentOption : _currentOption + 1;
                }
            }
        }

        // TODO : 사운드, 그래픽, 언어 등
        public void OnSelect(InputAction.CallbackContext context) // Z, Enter
        {
            if (context.started == true && IsEnabled == true)
            {
                switch (_currentOption)
                {
                    case SettingOptions.Sound:
                        {
                            Debug.Log("Sound");
                        }
                        break;
                    case SettingOptions.Graphics:
                        {
                            Debug.Log("Graphics");
                        }
                        break;
                    case SettingOptions.Other:
                        {
                            Debug.Log("Other");
                        }
                        break;
                }
            }
        }

        public void OnDeselect(InputAction.CallbackContext context) // X, Esc
        {
            if (context.started == true && IsEnabled == true)
            {
                ToggleUI(false);
            }
        }
    }
}
