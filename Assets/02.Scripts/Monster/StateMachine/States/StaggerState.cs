using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Monster.FSM
{
    public class StaggerState : StateBase
    {
        public StaggerState(StateMachine machine, IInputCommand inputCommand) : base(machine, inputCommand)
        {
            _staggerHash = Animator.StringToHash("Stagger");
        }

        public override State monsterState => State.Stagger;
        private readonly int _staggerHash;


        public override IEnumerator OnEnter(params object[] parameters)
        {
            // 이동 애니매이션으로 전환 실행.
            animator.Play(_staggerHash);
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
                (() => IsAnimationComplete(_staggerHash), State.Idle)
            };
        }
    }
}
