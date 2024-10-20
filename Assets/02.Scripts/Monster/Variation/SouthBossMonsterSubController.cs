using UnityEngine;
using System.Collections;
using FoxHill.Monster.AI;
using UnityEditor.Experimental.GraphView;
using Unity.Transforms;
using System;
using FoxHill.Core;
using FoxHill.Core.Pause;

namespace FoxHill.Monster.AI
{
    public class SouthBossMonsterSubController : MonoBehaviour, IPausable
    {
        public event Action<AnimationEvent> OnJumpAttack;
        [SerializeField] Animator _animator;
        private int _jumpHash = Animator.StringToHash("Jump");
        private bool _isPaused;
        private Vector2 _originPosition => (Vector2)transform.parent.position;


        private void Awake()
        {
            PauseManager.Register(this);
        }

        private void OnDestroy()
        {
            PauseManager.Unregister(this);
        }

        public void PlayAnimation()
        {
            _animator.Play(_jumpHash);
            if (_animator.isActiveAndEnabled)
            {
                DebugFox.Log("애니메이션 실행!");
            }
        }

        public void OnJumpUp() // 애니메이션 이벤트로 실행되는 메서드.
        {
            StartCoroutine(MoveDirectionExecutor(Vector2.up, 0.5f));//이동및 스프라이트 렌더러 비활성 처리.
        }

        public void OnJumpDown() // 애니메이션 이벤트로 실행되는 메서드.
        {

            StartCoroutine(LerpPositionExecutor(_originPosition, 0.5f));//이동및 스프라이트 렌더러 비활성 처리.
        }

        private IEnumerator MoveDirectionExecutor(Vector2 direction, float time)
        {
            float elapsedTime = 0;
            while (time > elapsedTime)
            {
                elapsedTime += Time.deltaTime;
                transform.position = (Vector2)transform.position + direction * 5f * elapsedTime;
                yield return null;
            }
        }

        private IEnumerator LerpPositionExecutor(Vector2 position, float duration)
        {
            position.y += 1f;
            float elapsedTime = 0;
            while (duration > elapsedTime)
            {
                elapsedTime += Time.deltaTime;

                float t = Mathf.Clamp01(elapsedTime / duration);
                if (Vector2.Distance(transform.position, position) < 0.1f)
                {
                    yield break;
                }
                transform.position = Vector2.Lerp(transform.parent.position, position, t);
                yield return null;
            }
        }

        public void PerformAttack(AnimationEvent animationEvent)
        {
            OnJumpAttack.Invoke(animationEvent);
        }

        public void Pause()
        {
            _isPaused = true;
            _animator.speed = 0f;
        }

        public void Resume()
        {
            _isPaused = false;
            _animator.speed = 1f;
        }
    }
}