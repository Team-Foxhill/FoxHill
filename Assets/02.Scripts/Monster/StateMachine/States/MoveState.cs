using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Monster.FSM
{
    public class MoveState : StateBase
    {
        public MoveState(StateMachine machine, IInputCommand inputCommand) : base(machine, inputCommand)
        {
            _digInHash = Animator.StringToHash("Dig_In");
            _digOutHash = Animator.StringToHash("Dig_Out");
            _moveHash = Animator.StringToHash("Move");
        }

        public override State monsterState => State.Move;
        private readonly int _digInHash;
        private readonly int _digOutHash;
        private readonly int _moveHash;


        public override IEnumerator OnEnter(params object[] parameters)
        {
            // 이동 애니매이션으로 전환 실행.
            animator.Play(_digInHash);
            yield return new WaitUntil(() => IsAnimationComplete(_digInHash));
            animator.Play(_moveHash);
        }

        public override IEnumerator OnExit()
        {
            animator.Play(_digOutHash);
            yield return new WaitUntil(() => IsAnimationComplete(_digOutHash));
        }

        protected override List<(Func<bool> condition, State next)> SetTransitions()
        {
            return new List<(Func<bool>, State)>(); // 업데이트 내에서 다른 애니메이션으로 전환할 이유가 없으므로 빈 리스트 반환.
        }
    }
}
