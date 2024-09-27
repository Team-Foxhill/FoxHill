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
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator _animator;
        [SerializeField] private PlayerManager _playerManager;

        private Dictionary<PlayerState, int> _animations = new Dictionary<PlayerState, int>(4); // {state, animationId}
        private bool _isPlayingInterruptibleAnimation = false;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _playerManager ??= GetComponentInParent<PlayerManager>();

            foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
            {
                _animations[state] = Animator.StringToHash(state.ToString());
            }
        }

        public void PlayAnimation(PlayerState state, bool isInterruptible)
        {
            if (_isPlayingInterruptibleAnimation == false)
            {
                return;
            }

            _animator.SetTrigger(_animations[state]);

            if (isInterruptible == true)
            {
                StartCoroutine(C_PlayInterruptibleAnimation());
            }
        }

        private IEnumerator C_PlayInterruptibleAnimation()
        {
            _isPlayingInterruptibleAnimation = true;

            float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
            float elapsedTime = 0f;

            while (elapsedTime < animationLength)
            {
                if (_playerManager.IsPaused == true)
                {
                    yield return new WaitUntil(() => _playerManager.IsPaused == false);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _isPlayingInterruptibleAnimation = false;
        }
    }
}
