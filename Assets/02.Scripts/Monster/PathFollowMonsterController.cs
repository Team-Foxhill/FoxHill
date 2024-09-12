using ProjectDawn.Navigation;
using System.Collections;
using UnityEngine;
using ProjectDawn.Navigation.Hybrid;
using Debugger = FoxHill.Core.Debugger;
using Unity.VisualScripting;
using System.Collections.Generic;
using System;


namespace FoxHill.Monster
{
    public class PathFollowMonsterController : MonsterBase
    {
        [SerializeField] private AgentAuthoring _agentAuthoring;
        [SerializeField] private AgentBody _agentBody;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _rotateUpdateInterval;
        [SerializeField] private Sprite[] _moveSpriteSet;
        [SerializeField] private Sprite[] _deadSpriteSet;
        [SerializeField] private Vector2 _teleportPosition = new Vector2(-55f, -70f);
        private readonly Vector3 _left = new Vector3(-1, 1, 1);
        private WaitForSecondsRealtime _waitTime;
        private WaitForSecondsRealtime _animationInterval;
        private WaitForSecondsRealtime _deadWait;
        private float _xVelocity;
        private bool _loop;


        private void Awake()
        {
            _agentAuthoring = GetComponent<AgentAuthoring>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _waitTime = new WaitForSecondsRealtime(_rotateUpdateInterval);
            _deadWait = new WaitForSecondsRealtime(1f);
        }

               private void OnEnable()
        {
            StartCoroutine(UpdateSprite(_moveSpriteSet, true));
            StartCoroutine(UpdateRotation());
        }

        // 단순히 move와 dead만 있는 경우이므로 bool값으로 코루틴 동작 정지 여부 결정.]
        private IEnumerator UpdateSprite(Sprite[] spriteSet, bool loop)
        {
            _loop = loop;
            float endFrame = spriteSet.Length;
            Debugger.Log(endFrame);
            _animationInterval = new WaitForSecondsRealtime(1 / endFrame);

            do
            {
                for (int i = 0; i < endFrame; i++)
                {
                    if (spriteSet != _deadSpriteSet && _loop == false) break;
                    _spriteRenderer.sprite = spriteSet[i];
                    yield return _animationInterval;
                }

            } while (_loop == true);
            yield break;
        }

        private IEnumerator UpdateRotation()
        {
            while (gameObject.activeSelf)
            {
                if (!_agentAuthoring.HasEntityBody)
                    yield break;

                _agentBody = _agentAuthoring.EntityBody;
                _xVelocity = _agentBody.Velocity.x;
                transform.localScale = _agentBody.Velocity.x > 0f ? Vector3.one : _left;
                yield return _waitTime;
            }
        }


        protected override void SetStat()
        {
            //파일에서 몬스터 종류 읽고 몬스터 스탯 가져오기.

        }

        public override void TakeDamage(float damage)
        {
            //프로퍼티 체력 깎기.
            CurrentHp -= damage;
            if (CurrentHp <= 0f) Dead();
        }

        public override void Dead()
        {
            _loop = false;
            StartCoroutine(PerformDie());
        }

        private IEnumerator PerformDie()
        {
            //죽음 애니메이션 실행시키기.
            yield return StartCoroutine(UpdateSprite(_deadSpriteSet, false));
            yield return _deadWait;
            // 안 보이는 곳으로 텔레포트
            transform.position = _teleportPosition;
            // HP 리셋.
            CurrentHp = MaxHp;
            gameObject.SetActive(false);
            yield break;
        }
    }
}
