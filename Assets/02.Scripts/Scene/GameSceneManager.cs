using FoxHill.Core.Pause;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoxHill.Scene
{
    public class GameSceneManager : MonoBehaviour, MenuInputAction.IGameSceneActions
    {
        [SerializeField] private GameSceneMenuController _menu;
        private MenuInputAction _inputAction;

        public void OnShowMenu(InputAction.CallbackContext context)
        {
            _menu.ToggleUI(true);
        }

        private void Awake()
        {
            _inputAction = new MenuInputAction();
            _inputAction.GameScene.AddCallbacks(this);
            _inputAction.GameScene.Enable();

            _menu ??= transform.Find("UI_Menu").GetComponent<GameSceneMenuController>();
            _menu.Initialize(_inputAction);
        }

        private void Start()
        {
            _menu.ToggleUI(false);
        }

        private void OnDestroy()
        {
            _inputAction?.Dispose();
        }
    }
}