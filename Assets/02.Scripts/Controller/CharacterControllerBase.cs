using FoxHill.Core;
using FoxHill.Core.Pause;
using FoxHill.States;
using UnityEngine;

namespace FoxHill.Controller
{
    [RequireComponent(typeof(Animator))]
    public abstract class CharacterControllerBase : MonoBehaviour, IPausable
    {
        protected bool _isPaused = false;
        protected bool _isDead = false;

        protected Animator _animator;
        protected State _state = State.Idle;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            PauseManager.Register(this);
        }

        protected virtual void OnDestroy()
        {
            PauseManager.Unregister(this);
        }

        protected virtual void ChangeState(State newState)
        {
            if (_state == newState)
                return;

            _animator.SetInteger(StateRepository.HASH_ID_STATE, (int)newState);
            _animator.SetBool(StateRepository.HASH_ID_IS_DIRTY, true); // SetBool(false)는 BehaviourBase에서 수행
        }

        public virtual void Pause()
        {
            _isPaused = true;
        }

        public virtual void Resume()
        {
            _isPaused = false;
        }
    }
}