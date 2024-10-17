using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

namespace FoxHill.Monster.FSM
{
    public class ChargeState : StateBase
    {
        public ChargeState(StateMachine machine, IInputCommand inputCommand) : base(machine, inputCommand)
        {
            _chargeHash = Animator.StringToHash("Attack_Charge");
        }

        public override State monsterState => State.Charge;
        private readonly int _chargeHash;
        private readonly int _stunHash;


        public override IEnumerator OnEnter(params object[] parameters)
        {
            animator.Play(_chargeHash);
            yield return new WaitUntil(() => IsAnimationComplete(_chargeHash));
        }

        public override IEnumerator OnExit()
        {
            yield break;
        }

        protected override List<(Func<bool> condition, State next)> SetTransitions()
        {
            return new List<(Func<bool>, State)>()
            {
             (() => IsAnimationComplete(_chargeHash), State.Jump)
            };
        }
    }
}
