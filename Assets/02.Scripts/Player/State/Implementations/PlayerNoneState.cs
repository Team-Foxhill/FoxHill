namespace FoxHill.Player.State.Implementations
{
    /// <summary>
    /// PlayerAction이 아무것도 지정되어 있지 않은 상태
    /// </summary>
    public class PlayerNoneState : PlayerStateBase
    {
        public override PlayerState State { get; protected set; } = PlayerState.None;
        public override bool IsMoveState { get; protected set; } = false;

        protected override void OnEnable()
        {
            // None state는 플레이할 애니메이션 존재하지 않음
        }
        protected override void OnDisable()
        {
            
        }
    }
}