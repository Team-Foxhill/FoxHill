using UnityEngine;

namespace FoxHill.Player.State.Implementations
{
    public class PlayerMoveState : PlayerStateBase
    {
        public override PlayerState State { get; protected set; } = PlayerState.Move;
        public override bool IsMoveState { get; protected set; } = true;

        private Transform _playerTransform;

        protected override void Awake()
        {
            base.Awake();
            _playerTransform = _manager.Transform;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _animator.PlayAnimation(State);
        }

        protected override void Update()
        {
            if(_manager.IsPaused == true)
            {
                return;
            }

            Vector2 movePosition = _manager.MoveInput * _manager.Stat.MoveSpeed * Time.deltaTime;
            _playerTransform.Translate(movePosition);
        }
    }
}