namespace FoxHill.Monster.FSM
{
    public enum State
    {
        None,
        Idle,
        Move,
        AttackHorizontal,
        AttackVertical,
        Jump,
        Charge,
        Stagger,
        KnockBack,
        AfterShock,
        Dead
    }
}
