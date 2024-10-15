using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Monster.FSM
{
    public class IdleState : StateBase
    {
        public IdleState(StateMachine machine, IInputCommand inputCommand) : base(machine, inputCommand)
        {
            _idleAnimationHash = Animator.StringToHash("Idle");
        }

        public override State monsterState => State.Idle;
        private readonly int _idleAnimationHash;


        public override IEnumerator OnEnter(params object[] parameters)
        {
            // Idle_Transition 실행.
            animator.Play(_idleAnimationHash);
            yield break;
        }

        public override IEnumerator OnExit()
        {
            yield break;
        }

        protected override List<(Func<bool>, State)> SetTransitions()
        {
            return new List<(Func<bool>, State)>();
        }
    }
}
