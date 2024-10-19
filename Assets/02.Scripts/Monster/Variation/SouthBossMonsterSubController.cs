using UnityEngine;
using System.Collections;
using FoxHill.Monster.AI;
using UnityEditor.Experimental.GraphView;
using Unity.Transforms;
using System;

namespace FoxHill.Monster.AI
{
    public class SouthBossMonsterSubController : MonoBehaviour
    {
        public event Action<AnimationEvent> OnJumpAttack;
        [SerializeField] Animator _animator;
        private int _jumpHash = Animator.StringToHash("Jump");
        private Vector2 _originPosition => (Vector2)transform.parent.position;

        public void PlayAnimation()
        {
            _animator.Play(_jumpHash);
        }

        public void OnJumpUp() // 애니메이션 이벤트로 실행되는 메서드.
        {
            StartCoroutine(MoveDirectionExecutor(Vector2.up, 0.5f));//이동및 스프라이트 렌더러 비활성 처리.
        }

        public void OnJumpDown() // 애니메이션 이벤트로 실행되는 메서드.
        {

            StartCoroutine(LerpPositionExecutor(_originPosition, 0.1f));//이동및 스프라이트 렌더러 비활성 처리.
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
                transform.position = Vector2.Lerp(transform.position, position, t);
                yield return null;
            }
        }

        public void PerformAttack(AnimationEvent animationEvent)
        {
            OnJumpAttack.Invoke(animationEvent);
        }
    }
}