using UnityEngine;
using UnityEngine.InputSystem;

namespace FoxHill.Player
{
    public class PlayerInputController : MonoBehaviour, PlayerInputAction.IPlayerActionActions, PlayerInputAction.IInventoryActionActions
    {
        [SerializeField] private PlayerManager _playerManager;

        private PlayerInputAction _inputAction;
        private Vector2 _moveInput;

        private void Awake()
        {
            if (_playerManager == null)
                _playerManager = GetComponentInParent<PlayerManager>();

            _inputAction = new PlayerInputAction();
            _inputAction.PlayerAction.AddCallbacks(this);
            _inputAction.InventoryAction.AddCallbacks(this);
            _inputAction.PlayerAction.Enable();
            _inputAction.InventoryAction.Disable();
        }

        private void Update()
        {
            if (_playerManager.IsPaused == true)
                return;

            if (_moveInput != Vector2.zero)
            {
                Move();
                _playerManager.Direction = _moveInput;
            }
        }


        #region InputAction Callback
        // PlayerAction
        public void OnAttack(InputAction.CallbackContext context) // V
        {
            Debug.Log("Attack");
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
        #endregion


        private void Move()
        {
            Vector2 movePosition = _moveInput * _playerManager.Stat.MoveSpeed * Time.deltaTime;
            _playerManager.CharacterController.Move(movePosition);
        }

        private void CastSkill()
        {
            if (_playerManager.IsPaused == true)
                return;

            _playerManager.OnCastSkill?.Invoke();
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
    }
}