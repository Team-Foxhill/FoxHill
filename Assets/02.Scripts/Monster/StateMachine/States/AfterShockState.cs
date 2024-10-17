using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Monster.FSM
{
    public class AfterShockState : StateBase
    {
        public AfterShockState(StateMachine machine, IInputCommand inputCommand) : base(machine, inputCommand)
        {
            _afterShockHash = Animator.StringToHash("Stun");
        }

        public override State monsterState => State.AfterShock;
        private readonly int _afterShockHash;


        public override IEnumerator OnEnter(params object[] parameters)
        {
            animator.Play(_afterShockHash);
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
                (() => IsAnimationComplete(_afterShockHash), State.Idle)
            };
        }
    }
}
