using FoxHill.Player.State;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Player
{
    /// <summary>
    /// Player의 애니메이션을 관리합니다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), (typeof(Animator)))]
    public class PlayerAnimationController : MonoBehaviour
    {
        public bool IsActionInProgress { get; private set; } = false;

        private readonly int HASH_DIRECTION_X = Animator.StringToHash("DirectionX");
        private readonly int HASH_DIRECTION_Y = Animator.StringToHash("DirectionY");

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private PlayerManager _playerManager;

        private Dictionary<PlayerMoveState, int> _moveAnimations = new Dictionary<PlayerMoveState, int>(3); // {state, animationId}
        private Dictionary<PlayerState, int> _actionAnimations = new Dictionary<PlayerState, int>(4); // {state, animationId}

        private PlayerMoveState _currentMoveState = PlayerMoveState.Idle;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playerManager ??= GetComponentInParent<PlayerManager>();

            foreach (PlayerMoveState state in Enum.GetValues(typeof(PlayerMoveState)))
            {
                _moveAnimations[state] = Animator.StringToHash(state.ToString());
            }

            foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
            {
                _actionAnimations[state] = Animator.StringToHash(state.ToString());
            }
        }

        private void Update()
        {
            SetDirection();
        }

        public void PlayAnimation(PlayerMoveState state)
        {
            _animator.Play(_moveAnimations[state]);
            _currentMoveState = state;
        }

        public void PlayAnimation(PlayerState state)
        {
            StartCoroutine(C_PlayAnimation(state));
        }

        private IEnumerator C_PlayAnimation(PlayerState state)
        {
            IsActionInProgress = true;

            _animator.Play(_actionAnimations[state]);

            var length = _animator.GetCurrentAnimatorStateInfo(0).length;

            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            IsActionInProgress = false;
        }

        /// <summary>
        /// Animator의 direction 파라미터 값을 설정합니다.
        /// 애니메이션이 대각선 기준으로 되어있으므로 위/아래 움직임 시 기존의 방향에 맞춰 움직이도록 합니다.
        /// </summary>
        private void SetDirection()
        {
            // 위/아래 이동
            if (_playerManager.Direction.x == 0 || _playerManager.Direction.y != 0)
            {
                float x = (_playerManager.IsLeftward == false) ? 1f : -1f;
                _animator.SetFloat(HASH_DIRECTION_X, x);
                _animator.SetFloat(HASH_DIRECTION_Y, _playerManager.Direction.y);
            }
            else
            {
                _animator.SetFloat(HASH_DIRECTION_X, _playerManager.Direction.x);
                _animator.SetFloat(HASH_DIRECTION_Y, _playerManager.Direction.y);
            }

        }
    }
}
