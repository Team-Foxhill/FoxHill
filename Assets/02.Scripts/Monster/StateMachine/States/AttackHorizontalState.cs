using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Monster.FSM
{
    public class AttackHorizontalState : StateBase
    {
        public AttackHorizontalState(StateMachine machine, IInputCommand inputCommand) : base(machine, inputCommand)
        {
            _attackHorizontalHash = Animator.StringToHash("Attack_Horizontal");
        }

        public override State monsterState => State.AttackHorizontal;
        private readonly int _attackHorizontalHash;


        public override IEnumerator OnEnter(params object[] parameters)
        {
            animator.Play(_attackHorizontalHash);
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
                (() => IsAnimationComplete(_attackHorizontalHash), State.Idle)
            };
        }
    }
}
