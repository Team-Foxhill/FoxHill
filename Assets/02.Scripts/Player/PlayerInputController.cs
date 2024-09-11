using FoxHill.Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoxHill.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInputController : MonoBehaviour, PlayerInputAction.IPlayerActionActions
    {
        private PlayerController _playerController;

        private PlayerInputAction _inputAction;
        private Vector2 _moveInput;

        private float _moveSpeed = 5f; // TODO : ºÐ¸®

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();

            _inputAction = new PlayerInputAction();
            _inputAction.PlayerAction.AddCallbacks(this);
            _inputAction.Enable();
        }

        private void Update()
        {
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

        public void OnMove(InputAction.CallbackContext context)
        {
            Debug.Log("Move");
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnSkill(InputAction.CallbackContext context)
        {
            Debug.Log("Skill");
        }

        public void OnSwap(InputAction.CallbackContext context)
        {
            Debug.Log("Swap");
        }

        public void OnTower(InputAction.CallbackContext context)
        {
            Debug.Log("Tower");
        }
        #endregion

        public void Move()
        {
            Vector2 movePosition = _moveInput * _moveSpeed * Time.deltaTime;
            _playerController.CharacterController.Move(movePosition);
        }
    }
}