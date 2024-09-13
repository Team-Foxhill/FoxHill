using UnityEngine;
using UnityEngine.InputSystem;

namespace FoxHill.Player
{
    public class PlayerInputController : MonoBehaviour, PlayerInputAction.IPlayerActionActions
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
            _inputAction.Enable();
        }

        private void Update()
        {
            if (_playerManager.IsPaused == true)
                return;

            if (_moveInput != Vector2.zero)
            {
                Move();
            }
        }

        #region InputAction Callback
        public void OnAttack(InputAction.CallbackContext context)
        {
            Debug.Log("Attack");
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            Debug.Log("Dodge");
        }

        public void OnMove(InputAction.CallbackContext context) // Arrow
        {
            Debug.Log("Move");

            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnCastSkill(InputAction.CallbackContext context) // B
        {
            if (context.started == true)
            {
                Debug.Log("CastSkill");
                CastSkill();
            }
        }

        public void OnSwitchSkill(InputAction.CallbackContext context) // X
        {
            if (context.started == true)
            {
                Debug.Log("SwitchSkill");
                SwitchSkill();
            }
        }

        public void OnTower(InputAction.CallbackContext context)
        {
            Debug.Log("Tower");
        }
        #endregion

        public void Move()
        {
            Vector2 movePosition = _moveInput * _playerManager.Stat.MoveSpeed * Time.deltaTime;
            _playerManager.CharacterController.Move(movePosition);
        }

        public void CastSkill()
        {
            _playerManager.OnCastSkill?.Invoke();
        }

        public void SwitchSkill()
        {
            _playerManager.OnSwitchSkill?.Invoke();
        }
    }
}