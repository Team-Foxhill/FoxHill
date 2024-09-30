using FoxHill.Core;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Player.State
{

    public enum PlayerState
    {
        Idle,
        Move,
        Attack,
        Skill,
        Dodge,
        Dead
    }

    /// <summary>
    /// Player의 상태를 관리합니다.
    /// </summary>
    [RequireComponent(typeof(PlayerAnimationController))]
    public class PlayerStateManager : MonoBehaviour
    {
        private PlayerAnimationController _animator;

        private Dictionary<PlayerState, PlayerStateBase> _states = new(6);

        private void Awake()
        {
            _animator = GetComponent<PlayerAnimationController>();

            foreach (PlayerStateBase state in transform.GetComponents<PlayerStateBase>())
            {
                _states.Add()
            }
        }

        public void SetState(PlayerMoveState newState)
        {
            if (newState == _currentMoveState)
            {
                return;
            }

            _currentMoveState = newState;

            if (_animator.IsActionInProgress == false) // 현재 진행중인 Action이 없으면 Move animation 적용
            {
                _animator.PlayAnimation(_currentMoveState);
            }
        }

        public void SetState(PlayerState newState)
        {
            if (_animator.IsActionInProgress == true) // 다른 action이 진행중이면 state 적용되지 않음
            {
                return;
            }

            _animator.PlayAnimation(newState);
        }
    }
}