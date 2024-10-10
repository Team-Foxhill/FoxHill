using FoxHill.Core.Pause;
using FoxHill.Core.Settings;
using FoxHill.UI.Buttons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class GameSceneMenuController : MonoBehaviour, MenuInputAction.IGameSceneMenuActions
{
    public bool IsEnabled => _canvas.enabled;

    private MenuInputAction _inputAction;

    [System.Serializable]
    private class MenuButton
    {
        public Button Button;
        private Action _action;
        [HideInInspector] public TitleButtonAnimation Animation;

        public void Initialize(Action action)
        {
            Animation = Button.GetComponent<TitleButtonAnimation>();
            _action = action;
        }

        public void Invoke()
        {
            _action?.Invoke();
        }
    }

    private enum Menu
    {
        Return,
        Exit
    }

    private Canvas _canvas;
    [SerializeField] private MenuButton _returnButton;
    [SerializeField] private MenuButton _exitButton;

    private Menu _currentMenu;
    private Dictionary<Menu, MenuButton> _menuButtons = new Dictionary<Menu, MenuButton>(3);

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();

        _returnButton.Initialize(() => ToggleUI(false));
        _exitButton.Initialize(() => StartCoroutine(C_ExitToTitleScene()));

        _menuButtons.Add(Menu.Return, _returnButton);
        _menuButtons.Add(Menu.Exit, _exitButton);
    }

    private void Start()
    {
        _currentMenu = Menu.Return;
        _menuButtons[_currentMenu].Animation.OnHoverEnter();
    }

    private void OnDestroy()
    {
        _inputAction?.Dispose();
    }

    public void Initialize(MenuInputAction inputaction)
    {
        _inputAction = inputaction;
        _inputAction.GameSceneMenu.AddCallbacks(this);
    }

    public void ToggleUI(bool toggle)
    {
        _canvas.enabled = toggle;

        if (toggle == true)
        {
            _inputAction.GameSceneMenu.Enable();
            OnHoverOptionEnter();
            PauseManager.Pause();
        }
        else
        {
            _inputAction.GameSceneMenu.Disable();
            _currentMenu = 0;
            OnHoverOptionExit();
            PauseManager.Resume();
        }
    }

    public void OnSwitchMenu(InputAction.CallbackContext context)
    {
        if (context.started == true && IsEnabled == true)
        {
            var input = context.ReadValue<Vector2>();

            OnHoverOptionExit();
            if (input == Vector2.up)
            {
                _currentMenu = (_currentMenu == 0) ? _currentMenu : _currentMenu - 1;
            }
            else if (input == Vector2.down)
            {
                _currentMenu = (_currentMenu == Menu.Exit) ? _currentMenu : _currentMenu + 1;
            }
            OnHoverOptionEnter();
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.started == true && IsEnabled == true)
        {
            _menuButtons[_currentMenu]?.Invoke();
        }
    }

    private void OnHoverOptionEnter()
    {
        _menuButtons[_currentMenu].Animation.OnHoverEnter();
    }

    private void OnHoverOptionExit()
    {
        _menuButtons[_currentMenu].Animation.OnHoverExit();
    }

    private IEnumerator C_ExitToTitleScene()
    {
        var progress = SceneManager.LoadSceneAsync("TitleScene");
        while (progress.isDone == false)
        {
            yield return null;
        }
    }
}
