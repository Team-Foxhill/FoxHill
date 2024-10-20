using FoxHill.Core;
using FoxHill.Core.Pause;
using FoxHill.Map;
using FoxHill.Player;
using FoxHill.Quest;
using FoxHill.Scene.Production;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace FoxHill.Scene
{
    public class GameSceneManager : MonoBehaviour, MenuInputAction.IGameSceneActions
    {
        [SerializeField] private GameSceneMenuController _menu;
        [SerializeField] private Canvas _deadUI;
        [SerializeField] private GameSceneProduction _production;
        private MenuInputAction _inputAction;
        [SerializeField] private PathFollowMonsterSpawner _pathFollowMonsterSpawner;
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private DestinationHPManager _destinationHPManager;

        public bool StartProductionOnLoad = false; // TODO : 지우기

        public void OnShowMenu(InputAction.CallbackContext context)
        {
            if (context.started == true)
            {
                _menu.ToggleUI(!_menu.IsEnabled);
            }
        }

        private void Awake()
        {
            _inputAction = new MenuInputAction();
            _inputAction.GameScene.AddCallbacks(this);
            _inputAction.GameScene.Enable();

            _menu ??= transform.Find("UI_Menu").GetComponent<GameSceneMenuController>();
            _menu.Initialize(_inputAction);

            _deadUI ??= transform.Find("UI_Dead").GetComponent<Canvas>();

            _production ??= transform.Find("Production").GetComponent<GameSceneProduction>();

            _pathFollowMonsterSpawner ??= FindFirstObjectByType<PathFollowMonsterSpawner>();
            _destinationHPManager ??= FindFirstObjectByType<DestinationHPManager>();
            _playerManager ??= FindFirstObjectByType<PlayerManager>();
        }

        private void Start()
        {
            _menu.ToggleUI(false);
            _deadUI.enabled = false;

            _pathFollowMonsterSpawner.Initialize();

            if (StartProductionOnLoad == true)
                StartCoroutine(C_SceneProduction());

            QuestManager.Reset();

            _playerManager.OnDead += ExitToTitleScene;
            _destinationHPManager.OnDead += ExitToTitleScene;
            _playerManager.OnReset?.Invoke();
        }

        private void OnDestroy()
        {
            _inputAction?.Dispose();

            _playerManager.OnDead -= ExitToTitleScene;
        }

        private IEnumerator C_SceneProduction()
        {
            PauseManager.Pause();
            yield return StartCoroutine(_production.C_StartSceneProduction());
            PauseManager.Resume();
        }

        private void ExitToTitleScene()
        {
            StartCoroutine(C_ExitToTitleScene());
        }

        private IEnumerator C_ExitToTitleScene()
        {
            _deadUI.enabled = true;

            float transitionTime = 5f;
            yield return new WaitForSeconds(transitionTime);

            SceneManager.LoadScene("TitleScene");            
        }
    }
}