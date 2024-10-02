namespace FoxHill.Player.State.Implementations
{
    public class PlayerIdleState : PlayerStateBase
    {
        public override PlayerState State { get; protected set; } = PlayerState.Idle;
        public override bool IsMoveState { get; protected set; } = true;
    }
}