using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Monster.FSM
{
    public class AttackVerticalState : StateBase
    {
        public AttackVerticalState(StateMachine machine, IInputCommand inputCommand) : base(machine, inputCommand)
        {
            _attackVerticalHash = Animator.StringToHash("Attack_Vertical");
        }

        public override State monsterState => State.AttackVertical;
        private readonly int _attackVerticalHash;


        public override IEnumerator OnEnter(params object[] parameters)
        {
            animator.Play(_attackVerticalHash);
            yield break;
        }

        public override IEnumerator OnExit()
        {
            yield break;
        }

        protected override List<(Func<bool> condition, State next)> SetTransitions()
        {
            return new List<(Func<bool>, State)>()
            {
                (() => IsAnimationComplete(_attackVerticalHash), State.Idle)
            };
        }
    }
}
