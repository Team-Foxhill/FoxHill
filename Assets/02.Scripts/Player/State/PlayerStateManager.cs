using FoxHill.Core;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace FoxHill.Player.State
{
    public enum PlayerState
    {
        Idle,
        Move,
        Attack,
        Dodge,
        Skill,
        Dead
    }

    /// <summary>
    /// Player의 상태를 관리하고, 상태에 따른 애니메이션을 적용합니다.
    /// </summary>
    [RequireComponent(typeof(PlayerAnimationController))]
    public class PlayerStateManager : MonoBehaviour
    {
        public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

        private bool _isMoving = false;

        private PlayerAnimationController _animator;

        private void Awake()
        {
            _animator = GetComponent<PlayerAnimationController>();
        }

        public void SetState(PlayerState newState)
        {
            if (newState == PlayerState.Idle || newState == PlayerState.Move)
            {
                // 이동 상태가 바뀌었다면 적용 후 메소드를 반환합니다.
                if (IsMoveStateChanged(newState) == true)
                {
                    SetMoveState(newState);
                    return;
                }
            }
            else if (newState == CurrentState)
            {
                return;
            }

            CurrentState = newState;

            if(_isMoving == true)
            {
                _animator.PlayAnimation(CurrentState, true);
            }
        }

        /// <summary>
        /// 플레이어의 움직임 상태 변경 여부를 확인합니다.
        /// </summary>
        /// <param name="newState">이동과 관련된 State를 입력받습니다. 예외 처리는 호출된 클래스에서 수행합니다.</param>
        /// <returns>상태가 변경되었으면 True, 그렇지 않으면 False를 반환합니다.</returns>
        private bool IsMoveStateChanged(PlayerState newState)
        {
            if (newState == PlayerState.Idle)
            {
                return (_isMoving == true);
            }
            else // PlayerState.Move
            {
                return (_isMoving == false);
            }
        }

        private void SetMoveState(PlayerState newState)
        {
            if (newState == PlayerState.Idle)
            {
                _isMoving = false;
            }
            else if (newState == PlayerState.Move)
            {
                _isMoving = true;
            }
            else
            {
                DebugFox.LogError($"Invalid MoveState : {newState}");
            }
        }
    }
}