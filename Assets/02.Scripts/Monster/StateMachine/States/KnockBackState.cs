using FoxHill.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Monster.FSM
{
    public class KnockBackState : StateBase
    {
        public KnockBackState(StateMachine machine, IInputCommand inputCommand) : base(machine, inputCommand)
        {
            _stunHash = Animator.StringToHash("Stun");
        }

        public override State monsterState => State.KnockBack;
        private readonly int _stunHash;


        public override IEnumerator OnEnter(params object[] parameters)
        {
            // 이동 애니매이션으로 전환 실행.
            animator.Play(_stunHash);
            DebugFox.Log($"{this} Enter");
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
                (() => IsAnimationComplete(_stunHash), State.AfterShock)
            };
        }
    }
}
