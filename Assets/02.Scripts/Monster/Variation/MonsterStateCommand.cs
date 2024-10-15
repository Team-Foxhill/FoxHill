using FoxHill.Monster.FSM;
using System;

namespace FoxHill.Monster.FSM
{
    internal class MonsterStateCommand : IInputCommand
    {
        public bool doJumpTrigger
        {
            get
            {
                if (_doJumpTrigger)
                {
                    _doJumpTrigger = false; // 읽을 때만 true를 반환하는 게 트리거다.
                    return true;
                }
                return false;
            }
            set
            {
                _doJumpTrigger = value;
            }
        }
        public bool doAttackTrigger
        {
            get
            {
                if (_doAttackTrigger)
                {
                    _doAttackTrigger = false;
                    return true;
                }
                return false;
            }
            set
            {
                _doAttackTrigger = value;
            }
        }

        public bool doIdleTrigger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool doMoveTrigger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool doChargeTrigger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool doStaggerTrigger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool doDeadTrigger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private bool _doJumpTrigger;
        private bool _doAttackTrigger;
    }
}
