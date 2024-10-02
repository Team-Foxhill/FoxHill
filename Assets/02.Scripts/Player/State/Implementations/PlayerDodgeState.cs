namespace FoxHill.Player.State.Implementations
{
    public class PlayerDodgeState : PlayerStateBase
    {
        public override PlayerState State { get; protected set; } = PlayerState.Dodge;
        public override bool IsMoveState { get; protected set; } = false;


        
    }
}
