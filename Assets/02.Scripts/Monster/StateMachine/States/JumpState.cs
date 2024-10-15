using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

namespace FoxHill.Monster.FSM
{
    public class JumpState : StateBase
    {
        public JumpState(StateMachine machine, IInputCommand inputCommand) : base(machine, inputCommand)
        {
            _jumpHash = Animator.StringToHash("Attack_Jump");
            _stunHash = Animator.StringToHash("Stun");
        }

        public override State monsterState => State.Jump;
        private readonly int _jumpHash;
        private readonly int _stunHash;


        public override IEnumerator OnEnter(params object[] parameters)
        {
            animator.Play(_jumpHash);
            yield return new WaitUntil(() => IsAnimationComplete(_jumpHash));
            animator.Play(_stunHash);
            yield return new WaitUntil(() => IsAnimationComplete(_stunHash));

        }

        public override IEnumerator OnExit()
        {
            yield break;
        }

        protected override List<(Func<bool> condition, State next)> SetTransitions()
        {
            return new List<(Func<bool>, State)>()
            {
                (() => IsAnimationComplete(_stunHash), State.Idle)
            };
        }
    }
}
