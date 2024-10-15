using UnityEngine;

namespace FoxHill.Player.State.Implementations
{
    public class PlayerDeadState : PlayerStateBase
    {
        public override PlayerState State { get; protected set; } = PlayerState.Dead;
        public override bool IsMoveState { get; protected set; } = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            _manager.GetComponent<CircleCollider2D>().enabled = false;
        }
    }
}