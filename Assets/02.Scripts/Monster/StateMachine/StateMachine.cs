using FoxHill.Core;
using FoxHill.Core.Pause;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace FoxHill.Monster.FSM
{
    public class StateMachine : MonoBehaviour, IPausable
    {
        public State CurrentState => _current.monsterState;
        public IState Current => _current;
        private IState _current;
        private IDictionary<State, IState> _states;
        private bool _isInitialized;
        private Coroutine _changeStateCoroutine;
        private bool _isTransitioning;
        private bool _isPaused;
        private Animator _animator;

        public void Initialize(IDictionary<State, IState> states)
        {
            _states = states;
            _isInitialized = true;
            _current = states.First().Value; // 딕셔너리의 첫 값을 가져옴.
            _animator = GetComponent<Animator>();
            PauseManager.Register(this);
        }

        private void OnDestroy()
        {
            PauseManager.Unregister(this);
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return; // 초기화되지 않았다면 메서드 실행 종료.
            }

            if (_isTransitioning)
            {
                return;
            }

            State next = _current.OnUpdate(); // 다음 스테이트를 현재 State의 OnUpdate메서드에서 반환받음.
            ChangeState(next); // 다음 스테이트로 변경하는 메서드 실행.
        }

        public bool ChangeState(State newState, params object[] parameters)
        {
            if (CurrentState == State.Dead)
            {
                return false;
            }
            if (_isTransitioning)
            {
                return false;
            }

            if (CurrentState == newState)
            {
                return false;
            }

            _isTransitioning = true;
            _changeStateCoroutine = StartCoroutine(ChangeStateCoroutine(CurrentState, newState, parameters));// 다음 프레임의 YIELD NULL에서 실행됨.
            return true;
        }

        private IEnumerator ChangeStateCoroutine(State from, State to, params object[] parameters)
        {
            while (_isPaused)
            {
                yield return new WaitUntil(() => _isPaused == false);
            }
            if (CurrentState == State.Dead)
            {
                yield break;
            }

            DebugFox.Log($"{_states[from].ToString()} Exit");
            yield return StartCoroutine(_states[from].OnExit());
            DebugFox.Log($"{_states[to].ToString()} Enter");
            yield return StartCoroutine(_states[to].OnEnter(parameters));
            _isTransitioning = false;
            _changeStateCoroutine = null;
            _current = _states[to];
        }

        public void Pause()
        {
            _isPaused = true;
            _animator.speed = 0;
            DebugFox.Log($"Paused, {gameObject.name}'s {_animator.name}'s speed is now {(float)_animator.speed}");
        }

        public void Resume()
        {
            _isPaused = false;
            _animator.speed = 1;
            DebugFox.Log($"Resumed, {gameObject.name}'s {_animator.name}'s speed is now {(float)_animator.speed}");
        }
    }
}
