using FoxHill.Core;
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
        private MenuInputAction _inputAction;

        [SerializeField] private PathFollowMonsterSpawner _pathFollowMonsterSpawner;

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

            _pathFollowMonsterSpawner ??= FindFirstObjectByType<PathFollowMonsterSpawner>();
        }

        private void Start()
        {
            _menu.ToggleUI(false);

            _pathFollowMonsterSpawner.Initialize();

            // StartCoroutine(C_SceneProduction());
        }

        private void OnDestroy()
        {
            _inputAction?.Dispose();
        }

        private IEnumerator C_SceneProduction()
        {
            PauseManager.Pause();
            yield return StartCoroutine(_production.C_StartSceneProduction());
            PauseManager.Resume();
        }
    }
}