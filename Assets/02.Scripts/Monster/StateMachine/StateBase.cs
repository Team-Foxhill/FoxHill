using FoxHill.Core.Pause;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FoxHill.Monster.FSM
{
    public abstract class StateBase : IState, IPausable
    {
        public StateBase(StateMachine machine, IInputCommand inputCommand)
        {
            this.inputCommand = inputCommand;
            transitions = SetTransitions();
            animator = machine.GetComponent<Animator>();
            PauseManager.Register(this);
        }

        ~StateBase()
        {
            PauseManager.Unregister(this);
        }

        public abstract State monsterState { get; }
        protected List<(Func<bool>, State)> transitions; // 컨디션과, 다음 상태를 가지고 있는 튜플. 유니티 애니메이터의 트랜지션 대신 사용.
        protected IInputCommand inputCommand;
        protected Animator animator;
        protected AnimatorStateInfo animatorStateInfo; // 자식 클래스에서 할당해줘야 함.
        protected bool _isPaused;

        public abstract IEnumerator OnEnter(params object[] parameters);

        public virtual State OnUpdate()
        {
            State next = this.monsterState; // 다음 스테이트를 지정.
            foreach (var transition in transitions)
            {
                if (transition.Item1.Invoke()) // 조건이 성립하는 첫 리스트 요소의.
                {
                    next = transition.Item2; // 스테이트를 다음 실행될 스테이트로 지정.
                    break;
                }
            }
            return next; // 다음 스테이트를 반환.
        }

        public abstract IEnumerator OnExit();

        protected abstract List<(Func<bool> condition, State next)> SetTransitions();

        protected bool IsAnimationComplete(int animationHash)
        {
            bool isAnimationComplete = false;
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            isAnimationComplete = (stateInfo.shortNameHash == animationHash) && (stateInfo.normalizedTime >= 1f);
            return isAnimationComplete;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }
    }
}
