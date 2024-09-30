using FoxHill.Player.State;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoxHill.Player
{
    public class PlayerInputController : MonoBehaviour,
        PlayerInputAction.IPlayerActionActions, PlayerInputAction.IInventoryActionActions, PlayerInputAction.ISpawnActionActions
    {
        private const float IDLE_TRANSITION_TIME = 3f;

        [SerializeField] private PlayerManager _playerManager;

        private PlayerInputAction _inputAction;
        private Vector2 _moveInput;
        private Vector2 _towerMoveInput;

        private float _elapsedTime = 0f;
        private bool _isIdle = false;

        private void Awake()
        {
            _playerManager ??= GetComponentInParent<PlayerManager>();

            _inputAction = new PlayerInputAction();

            _inputAction.PlayerAction.AddCallbacks(this);
            _inputAction.InventoryAction.AddCallbacks(this);
            _inputAction.SpawnAction.AddCallbacks(this);

            _inputAction.PlayerAction.Enable();
            _inputAction.InventoryAction.Disable();
            _inputAction.SpawnAction.Disable();
        }

        private void Start()
        {
            _playerManager.Inventory.OnUseConstructiveItem?.AddListener(_ => ToggleTowerSpawnMode(true));
            _playerManager.OnExitSpawn?.AddListener(() => ToggleTowerSpawnMode(false));
        }

        private void Update()
        {
            if (_towerMoveInput != Vector2.zero)
            {
                MoveTower();
            }

            // 이 구문 상단에 위치한 코드는 일시정지여도 실행할 수 있는 행동, 하단은 실행할 수 없는 행동
            if (_playerManager.IsPaused == true)
                return;

            if (_moveInput != Vector2.zero)
            {
                _isIdle = false;
                _elapsedTime = 0f;

                _playerManager.Direction = _moveInput;
                Move();
            }
            else
            {
                _elapsedTime += Time.deltaTime;

                if (_elapsedTime > IDLE_TRANSITION_TIME && _isIdle == false)
                {
                    Idle();
                }
            }
        }

        private void OnDestroy()
        {
            _inputAction?.Dispose();
        }


        #region InputAction Callback
        // PlayerAction
        public void OnAttack(InputAction.CallbackContext context) // V
        {
            if (context.started == true)
            {
                _isIdle = false;

                _playerManager.SetState(PlayerState.Attack);
            }
        }

        public void OnDodge(InputAction.CallbackContext context) // Z
        {
            Debug.Log("Dodge");
        }

        public void OnMove(InputAction.CallbackContext context) // Arrow
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnCastSkill(InputAction.CallbackContext context) // B
        {
            if (context.started == true)
            {
                _isIdle = false;

                CastSkill();
            }
        }

        public void OnSwitchSkill(InputAction.CallbackContext context) // X
        {
            if (context.started == true)
            {
                SwitchSkill();
            }
        }

        public void OnInventory(InputAction.CallbackContext context) // F
        {
            if (context.started == true)
            {
                ToggleInventory(true);
            }
        }


        // InventoryAction
        public void OnSwitchSlot(InputAction.CallbackContext context) // Arrow
        {
            if (context.performed == true)
            {
                _playerManager.OnSwitchSlotInventory?.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnClose(InputAction.CallbackContext context) // F
        {
            if (context.started == true)
            {
                ToggleInventory(false);
            }
        }
        public void OnSelect(InputAction.CallbackContext context) // Z
        {
            if (context.started == true)
            {
                _playerManager.OnSelectInventory?.Invoke();
            }
        }

        public void OnDeselect(InputAction.CallbackContext context) // X
        {
            if (context.started == true)
            {
                _playerManager.OnDeselectInventory?.Invoke();
            }
        }

        // SpawnAction
        public void OnMovePrefab(InputAction.CallbackContext context) // Arrow
        {
            _towerMoveInput = context.ReadValue<Vector2>();
        }

        public void OnConfirm(InputAction.CallbackContext context) // Z
        {
            if (context.started == true)
            {
                _playerManager.OnConfirmSpawn?.Invoke();
            }
        }

        public void OnCancel(InputAction.CallbackContext context) // X
        {
            if (context.started == true)
            {
                ToggleSpawn(false);
                _playerManager.OnCancelSpawn?.Invoke();
            }
        }

        #endregion
        private void Idle()
        {
            _isIdle = true;

            _playerManager.SetState(State.PlayerMoveState.Idle);
        }

        private void Move()
        {
            Vector2 movePosition = _moveInput * _playerManager.Stat.MoveSpeed * Time.deltaTime;
            _playerManager.CharacterController.Move(movePosition);

            _playerManager.SetState(State.PlayerMoveState.Move);
        }


        private void CastSkill()
        {
            if (_playerManager.IsPaused == true)
                return;

            _playerManager.OnCastSkill?.Invoke();
            _playerManager.SetState(PlayerState.Skill);
        }

        private void SwitchSkill()
        {
            if (_playerManager.IsPaused == true)
                return;

            _playerManager.OnSwitchSkill?.Invoke();
        }

        /// <summary>
        /// 타당성 검사 수행 이후 인벤토리를 끄고 켭니다.
        /// Input ActionMap을 스위칭합니다.
        /// </summary>
        /// <param name="toggle">True면 인벤토리를 켜고, False면 인벤토리를 끕니다.</param>
        private void ToggleInventory(bool toggle)
        {
            if (toggle == true && _playerManager.IsInventoryOpen == false)
            {
                _playerManager.OnOpenInventory?.Invoke();
                _inputAction.PlayerAction.Disable();
                _inputAction.InventoryAction.Enable();
            }

            else if (toggle == false && _playerManager.IsInventoryOpen == true)
            {
                _playerManager.OnCloseInventory?.Invoke();
                _inputAction.InventoryAction.Disable();
                _inputAction.PlayerAction.Enable();
            }
        }

        private void ToggleSpawn(bool toggle)
        {
            if (toggle == false && _playerManager.IsInventoryOpen == true)
            {
                _inputAction.SpawnAction.Disable();
                _inputAction.InventoryAction.Enable();
            }

            else if (toggle == true && _playerManager.IsInventoryOpen == true)
            {
                _inputAction.InventoryAction.Disable();
                _inputAction.SpawnAction.Enable();
            }
        }

        public void ToggleTowerSpawnMode(bool toggle)
        {
            if (_playerManager.IsInventoryOpen == false)
            {
                return;
            }

            if (toggle == true)
            {
                _inputAction.InventoryAction.Disable();
                _inputAction.SpawnAction.Enable();
            }

            if (toggle == false)
            {
                _inputAction.SpawnAction.Disable();
                _inputAction.InventoryAction.Enable();
            }
        }

        private void MoveTower()
        {
            _playerManager.OnMovePrefabSpawn?.Invoke(_towerMoveInput);
        }

    }
}