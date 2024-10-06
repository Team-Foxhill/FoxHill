using FoxHill.Player.State;
using System;
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
        public float AnimationTime
        {
            get => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        private readonly int HASH_DIRECTION_X = Animator.StringToHash("DirectionX");
        private readonly int HASH_DIRECTION_Y = Animator.StringToHash("DirectionY");

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private PlayerManager _playerManager;

        private Dictionary<PlayerState, int> _animations = new Dictionary<PlayerState, int>(4); // {state, animationId}

        private bool _isInitialized = false;
        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            SetDirection();
        }

        public void PlayAnimation(PlayerState state)
        {

            if (_animations.TryGetValue(state, out int stateHash) == true)
            {
                _animator.Play(stateHash,0,0f);
            }
        }

        public void FlipSprite(bool flip)
        {
            try
            {
                _spriteRenderer.flipX = flip;
            }
            catch (NullReferenceException ex)
            {
                Initialize();
                _spriteRenderer.flipX = flip;
            }
        }

        private void Initialize()
        {
            if (_isInitialized == true)
            {
                return;
            }

            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playerManager ??= GetComponentInParent<PlayerManager>();

            foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
            {
                _animations[state] = Animator.StringToHash(state.ToString());
            }

            _isInitialized = true;
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
