namespace FoxHill.Player.State.Implementations
{
    public class PlayerDeadState : PlayerStateBase
    {
        public override PlayerState State { get; protected set; } = PlayerState.Dead;
        public override bool IsMoveState { get; protected set; } = false;
    }
}