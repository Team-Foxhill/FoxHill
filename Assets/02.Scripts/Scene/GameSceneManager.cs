using FoxHill.Core.Pause;
using FoxHill.Player;
using FoxHill.Scene.Production;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoxHill.Scene
{
    public class GameSceneManager : MonoBehaviour, MenuInputAction.IGameSceneActions
    {
        [SerializeField] private GameSceneMenuController _menu;
        [SerializeField] private GameSceneProduction _production;
        [SerializeField] private PlayerManager _playerManager;
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

            _production ??= transform.Find("Production").GetComponent<GameSceneProduction>();

            _playerManager ??= FindFirstObjectByType<PlayerManager>();
        }

        private void Start()
        {
            _menu.ToggleUI(false);

            // StartCoroutine(C_SceneProduction());
        }

        private void OnDestroy()
        {
            _inputAction?.Dispose();
        }

        private IEnumerator C_SceneProduction()
        {
            _playerManager.Pause();
            yield return StartCoroutine(_production.C_StartSceneProduction());
            _playerManager.Resume();
        }
    }
}