using FoxHill.States;
using UnityEngine;

namespace FoxHill.Controller
{
    [RequireComponent(typeof(Animator))]
    public abstract class CharacterControllerBase : MonoBehaviour
    {
        protected Animator _animator;
        protected State _state = State.Idle;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        protected virtual void ChangeState(State newState)
        {
            if (_state == newState)
                return;

            _animator.SetInteger(StateRepository.HASH_ID_STATE, (int)newState);
            _animator.SetBool(StateRepository.HASH_ID_IS_DIRTY, true); // SetBool(false)는 BehaviourBase에서 수행
        }
    }
}