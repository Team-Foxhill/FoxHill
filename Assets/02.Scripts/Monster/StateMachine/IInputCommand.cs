namespace FoxHill.Monster.FSM
{
    public interface IInputCommand // 플레이어도 몬스터도 받아야 할 필요가 있을 수 있으므로.
    {
        public bool doIdleTrigger { get; set; }
        public bool doMoveTrigger { get; set; }
        public bool doJumpTrigger { get; set; }
        public bool doChargeTrigger { get; set; }
        public bool doAttackTrigger { get; set; }
        public bool doStaggerTrigger { get; set; }
        public bool doDeadTrigger { get; set; }

    }
}