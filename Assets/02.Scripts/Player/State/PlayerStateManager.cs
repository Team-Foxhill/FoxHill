using FoxHill.Core;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Player.State
{
    public enum PlayerState
    {
        // Move State
        Idle,
        Move,
        // Action State
        None,
        Attack,
        Dodge,
        Dead
    }

    /// <summary>
    /// Player의 상태를 관리합니다.
    /// </summary>

    public class PlayerStateManager : MonoBehaviour
    {
        private Dictionary<PlayerState, PlayerStateBase> _stateDictionary = new Dictionary<PlayerState, PlayerStateBase>(6);

        private PlayerState _currentMoveState = PlayerState.Idle;
        private PlayerState _currentActionState = PlayerState.None;

        private void Awake()
        {
            foreach (PlayerStateBase state in transform.GetComponents<PlayerStateBase>())
            {
                _stateDictionary.Add(state.State, state);
            }
        }

        private void Start()
        {
            _stateDictionary[_currentMoveState].enabled = true;
            _stateDictionary[_currentActionState].enabled = true;
        }

        private void Update()
        {
            if (_stateDictionary[_currentActionState].IsDone == true)
            {
                SetState(PlayerState.None);
                _stateDictionary[_currentMoveState].PlayAnimation();
            }
        }

        public void SetState(PlayerState state)
        {
            if (!_stateDictionary.TryGetValue(state, out var newState))
            {
                DebugFox.LogError($"State {state} not found in stateDictionary");
                return;
            }

            // 이전 State와 같으면 무시
            if (state == _currentMoveState || state == _currentActionState)
            {
                return;
            }

            if(newState.IsMoveState == true)
            {
                _stateDictionary[_currentMoveState].enabled = false;
                _currentMoveState = state;
            }
            else
            {
                _stateDictionary[_currentActionState].enabled = false;
                _currentActionState = state;
            }

            newState.enabled = true;
        }
    }
}