using FoxHill.Core;
using FoxHill.Core.Pause;
using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace FoxHill.Monster.AI
{
    public class SouthBossMonsterSubController : MonoBehaviour, IPausable
    {
        public event Action<AnimationEvent> OnJumpAttack;
        [SerializeField] Animator _animator;
        [SerializeField] SpriteRenderer _spriteRenderer;
        private int _jumpHash = Animator.StringToHash("Jump");
        private bool _isPaused;
        private Vector2 _originPosition;
        private Blackboard _blackboard;
        private bool _isStagger;


        private void Awake()
        {
            PauseManager.Register(this);
            StartCoroutine(C_GetComponent());
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private IEnumerator C_GetComponent()
        {
            while(_blackboard == default)
            {
                if (transform.parent.TryGetComponent<MonsterBehaviourTree>(out MonsterBehaviourTree monsterBehaviourTree))
                {
                    _blackboard = monsterBehaviourTree.Blackboard;
                    yield break;
                }
                yield return null;
            }
        }

        public void FlipX(bool isFlipped)
        {
            _spriteRenderer.flipX = isFlipped;
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
            StartCoroutine(C_MoveDirectionExecutor(Vector2.up, 0.5f));//이동및 스프라이트 렌더러 비활성 처리.
        }

        public void OnJumpDown() // 애니메이션 이벤트로 실행되는 메서드.
        {
            _originPosition = _blackboard.Target.position;
            StartCoroutine(C_LerpPositionExecutor(_originPosition, 1f));//이동및 스프라이트 렌더러 비활성 처리.
        }

        private IEnumerator C_MoveDirectionExecutor(Vector2 direction, float time)
        {
            float elapsedTime = 0;
            while (time > elapsedTime)
            {
                elapsedTime += Time.deltaTime;
                transform.position = (Vector2)transform.position + direction * 5f * elapsedTime;
                yield return null;
            }
        }

        private IEnumerator C_LerpPositionExecutor(Vector2 position, float duration)
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