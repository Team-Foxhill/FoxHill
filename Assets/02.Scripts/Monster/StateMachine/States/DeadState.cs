using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Monster.FSM
{
    public class DeadState : StateBase
    {
        public DeadState(StateMachine machine, IInputCommand inputCommand) : base(machine, inputCommand)
        {
            _deathHash = Animator.StringToHash("Dead"); // "Dead"는 애니메이터에서 설정한 죽음 애니메이션의 이름.
        }

        public override State monsterState => State.Dead;
        private readonly int _deathHash;


        public override IEnumerator OnEnter(params object[] parameters)
        {
            // 이동 애니매이션으로 전환 실행.
            animator.Play(_deathHash);
            bool temp = true;
            while (temp == true)
            {
                if (IsAnimationComplete(_deathHash))
                {
                    animator.speed = 0;
                    temp = false;
                }
                yield return null;
            }
        }

        public override IEnumerator OnExit()
        {
            yield break;
        }

        protected override List<(Func<bool>, State)> SetTransitions()
        {
            // 빈 리스트를 반환해서 더는 다른 애니메이션으로 변환될 수 없도록 처리.
            return new List<(Func<bool>, State)>();
        }
    }
}
