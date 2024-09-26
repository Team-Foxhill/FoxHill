using ProjectDawn.Navigation;
using ProjectDawn.Navigation.Hybrid;
using System;
using System.Collections;
using UnityEngine;
using DebugFox = FoxHill.Core.DebugFox;
using IPoolable = FoxHill.Core.IPoolable;


namespace FoxHill.Monster
{
    public class PathFollowMonsterController : MonsterBase, IPoolable
    {
        public event Action<IPoolable> OnRelease;
        [SerializeField] private int _monsterIndexNumber;
        [SerializeField] private AgentAuthoring _agentAuthoring;
        [SerializeField] private AgentBody _agentBody;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _rotateUpdateInterval = 2f;
        [SerializeField] private Sprite[] _moveSpriteSet;
        [SerializeField] private Sprite[] _deadSpriteSet;
        [SerializeField] private Vector2 _teleportPosition = new Vector2(-55f, -70f);
        private readonly Vector3 _left = new Vector3(-1, 1, 1);
        private WaitForSecondsRealtime _waitTime;
        private WaitForSecondsRealtime _animationInterval;
        private float _xVelocity;
        private bool _loop;


        private void Start()
        {
            _waitTime = new WaitForSecondsRealtime(_rotateUpdateInterval);
            MonsterDataManager.TryGetMonster(_monsterIndexNumber, out _monsterForm);
            StartCoroutine(WaitForComponents());
            SetStat();
            StartCoroutine(UpdateSprite(_moveSpriteSet, true));
        }
        protected override void SetStat()
        {
            base.SetStat();
            var Locomotion = _agentAuthoring.EntityLocomotion;
            Locomotion.Speed = MoveSpeed;
            _agentAuthoring.EntityLocomotion = Locomotion;
        }


        private void OnEnable()
        {
            StartCoroutine(WaitForComponents());
            StartCoroutine(UpdateSprite(_moveSpriteSet, true));
            StartCoroutine(UpdateRotation());
        }

        private IEnumerator WaitForComponents()
        {
            while (_agentAuthoring == default || _spriteRenderer == default)
            {
                if (_agentAuthoring == default)
                {
                    _agentAuthoring = GetComponent<AgentAuthoring>();
                }
                if (_spriteRenderer == default)
                {
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                }
                if (_agentAuthoring != default && _spriteRenderer != default)
                {
                    DebugFox.Log("All components Found.");
                    yield break;
                }
                DebugFox.Log("Some Component Not Found.");
                yield return _animationInterval;
            }
        }

        // 단순히 move와 dead만 있는 경우이므로 bool값으로 코루틴 동작 정지 여부 결정.]
        private IEnumerator UpdateSprite(Sprite[] spriteSet, bool loop)
        {
            _loop = loop;
            float endFrame = spriteSet.Length;
            DebugFox.Log(endFrame);
            _animationInterval = new WaitForSecondsRealtime(1 / endFrame);

            do
            {
                for (int i = 0; i < endFrame; i++)
                {
                    if (_spriteRenderer.enabled == false)
                    {
                        yield return new WaitUntil(() => _spriteRenderer.enabled == true);
                    }
                    if (_isPaused == true)
                    {
                        _agentAuthoring.enabled = false;
                        yield return new WaitUntil(() => _isPaused == false);
                        _agentAuthoring.enabled = true;
                    }
                    if (spriteSet != _deadSpriteSet && _loop == false) break;
                    _spriteRenderer.sprite = spriteSet[i];
                    yield return _animationInterval;
                }

            } while (_loop == true);
            yield break;
        }

        private IEnumerator UpdateRotation()
        {
            while (!_agentAuthoring.HasEntityBody)
            {
                yield return null;
            }

            while (gameObject.activeSelf)
            {
                if (_isPaused == true)
                {
                    _agentAuthoring.enabled = false;
                    yield return new WaitUntil(() => _isPaused == false);
                    _agentAuthoring.enabled = true;
                }
                _agentBody = _agentAuthoring.EntityBody;
                _xVelocity = _agentBody.Velocity.x;
                transform.localScale = _agentBody.Velocity.x > 0f ? Vector3.one : _left;

                yield return _waitTime;
            }
        }

        public override void Dead()
        {
            base.Dead();
            _loop = false;
            StartCoroutine(PerformDie());
        }

        private IEnumerator PerformDie()
        {
            //죽음 애니메이션 실행시키기.
            yield return StartCoroutine(UpdateSprite(_deadSpriteSet, false));
            // 풀에 되돌리기.
            OnRelease?.Invoke(this);
            yield break;
        }
    }
}
