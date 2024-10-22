using FoxHill.Audio;
using FoxHill.Core.Settings;
using FoxHill.UI.Buttons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FoxHill.UI.Menu
{
    /// <summary>
    /// TitleScene에서의 메뉴를 관리합니다.
    /// </summary>
    public class TitleSceneMenuController : MonoBehaviour, MenuInputAction.ITitleSceneActions, IVolumeAdjustable
    {
        [SerializeField] private MenuInputAction _inputAction;

        [SerializeField] private TitleButton _startButton;
        [SerializeField] private TitleButton _settingsButton;
        [SerializeField] private TitleButton _exitButton;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _keySound;


        [System.Serializable]
        private class TitleButton
        {
            public Button Button;
            [HideInInspector] public TitleButtonAnimation Animation;

            public void Initialize()
            {
                Animation = Button.GetComponent<TitleButtonAnimation>();
            }
        }

        [SerializeField] private GameSettingsManager _settingsManager;

        private enum Menu
        {
            Start,
            Settings,
            Exit
        }

        private Menu _currentMenu;

        private Dictionary<Menu, TitleButton> _menuButtons = new Dictionary<Menu, TitleButton>(3);

        private void Awake()
        {
            SoundVolumeManager.Register(this);
            _inputAction ??= new MenuInputAction();

            _inputAction.TitleScene.AddCallbacks(this);
            _inputAction.TitleScene.Enable();

            _settingsManager ??= FindFirstObjectByType<GameSettingsManager>();

            _settingsManager.InputAction = _inputAction;
            _inputAction.TitleSceneSettings.AddCallbacks(_settingsManager);

            _startButton.Initialize();
            _settingsButton.Initialize();
            _exitButton.Initialize();

            _menuButtons.Add(Menu.Start, _startButton);
            _menuButtons.Add(Menu.Settings, _settingsButton);
            _menuButtons.Add(Menu.Exit, _exitButton);
        }

        private void Start()
        {
            _currentMenu = Menu.Start;
            _menuButtons[_currentMenu].Animation.OnHoverEnter();
            _settingsManager.ToggleUI(false);
        }

        private void OnDestroy()
        {
            SoundVolumeManager.Unregister(this);
            _inputAction?.Dispose();
        }


        #region InputAction Callbacks
        public void OnSwitchMenu(InputAction.CallbackContext context) // Arrow
        {
            if (context.started == true && _settingsManager.IsEnabled == false)
            {
                var input = context.ReadValue<Vector2>();

                PlayKeySound(0);
                _menuButtons[_currentMenu].Animation.OnHoverExit();
                if (input == Vector2.up)
                {
                    _currentMenu = (_currentMenu == Menu.Start) ? _currentMenu : _currentMenu - 1;
                }
                else if (input == Vector2.down)
                {
                    _currentMenu = (_currentMenu == Menu.Exit) ? _currentMenu : _currentMenu + 1;
                }
                _menuButtons[_currentMenu].Animation.OnHoverEnter();
            }
        }

        public void OnSelect(InputAction.CallbackContext context) // Z, Enter
        {
            if (context.started == true && _settingsManager.IsEnabled == false)
            {
                PlayKeySound(1);
                switch (_currentMenu)
                {
                    case Menu.Start:
                        {
                            StartGame();
                        }
                        break;
                    case Menu.Settings:
                        {
                            _settingsManager.ToggleUI(true);
                        }
                        break;
                    case Menu.Exit:
                        {
                            ExitGame();
                        }
                        break;
                }
            }
        }

        public void OnDeselect(InputAction.CallbackContext context) // X, Esc
        {
                PlayKeySound(2);
            return;
        }
        #endregion


        private void StartGame()
        {
            StartCoroutine(C_StartGame());
        }

        private IEnumerator C_StartGame()
        {
            var progress = SceneManager.LoadSceneAsync("GameSceneTest");
            while(progress.isDone == false)
            {
                yield return null;
            }
        }

        private void PlayKeySound(int id)
        {

            if (_audioSource != null && _keySound[id] != null)
            {
                _audioSource.PlayOneShot(_keySound[id]);
            }
        }

        public void OnVolumeChanged(float volume)
        {
            _audioSource.volume = volume;
        }

        private void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
