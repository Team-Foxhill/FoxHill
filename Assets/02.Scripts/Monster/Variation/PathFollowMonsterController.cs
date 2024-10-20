using FoxHill.Core.Damage;
using FoxHill.Items;
using ProjectDawn.Navigation;
using ProjectDawn.Navigation.Hybrid;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using IPoolable = FoxHill.Core.IPoolable;
using FoxHill.Audio;


namespace FoxHill.Monster
{
    public class PathFollowMonsterController : MonsterBase, IPoolable, IVolumeAdjustable
    {
        public event Action<IPoolable> OnRelease;
        [SerializeField] private int _monsterIndexNumber;
        [SerializeField] private AgentAuthoring _agentAuthoring;
        [SerializeField] private AgentBody _agentBody;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _rotateUpdateInterval = 2f;
        [SerializeField] private Sprite[] _moveSpriteSet;
        [SerializeField] private Sprite[] _damagedSpriteSet;
        [SerializeField] private Sprite[] _deadSpriteSet;
        [SerializeField] private AudioClip[] _audioClips;
        [SerializeField] private Vector2 _teleportPosition = new Vector2(-55f, -70f);
        [SerializeField] private float _colorChangeWait = 0.2f;

        private readonly Color COLOR_DAMAGED = new Color(255f / 255f, 47f / 255f, 47f / 255f);
        private readonly Vector3 _left = new Vector3(-1, 1, 1);

        private WaitForSecondsRealtime _waitTime;
        private WaitForSecondsRealtime _animationInterval;
        private float _xVelocity;
        private bool _loop;
        private Color _initialColor;
        private bool _isDamaged;
        private bool _isCheckElapsedTime;
        private float _elapsedTime;
        private float _startTime;
        private AudioSource _audioSource;
        private Coroutine _deadCoroutine;



        private void Start()
        {
            _initialColor = _spriteRenderer.color;
            _waitTime = new WaitForSecondsRealtime(_rotateUpdateInterval);
            MonsterDataManager.TryGetMonster(_monsterIndexNumber, out _monsterForm);
            StartCoroutine(WaitForComponents());
            SetStat();
            _spriteRenderer.enabled = false;
            StartCoroutine(UpdateSprite(_moveSpriteSet, true));
            SoundVolumeManager.Register(this);
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
            CurrentHp = MaxHp;
            _agentAuthoring.enabled = true;
            _deadCoroutine = null;
        }

        private void OnDestroy()
        {
            SoundVolumeManager.Unregister(this);
        }

        private IEnumerator WaitForComponents()
        {
            while (_agentAuthoring == default || _spriteRenderer == default || _audioSource == default)
            {
                if (_agentAuthoring == default)
                {
                    _agentAuthoring = GetComponent<AgentAuthoring>();
                }
                if (_spriteRenderer == default)
                {
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                }
                if (_audioSource == default)
                {
                    _audioSource = GetComponent<AudioSource>();
                }
                if (_agentAuthoring != default && _spriteRenderer != default)
                {
                    yield break;
                }
                yield return _animationInterval;
            }
        }

        // 단순히 move와 dead만 있는 경우이므로 bool값으로 코루틴 동작 정지 여부 결정.]
        private IEnumerator UpdateSprite(Sprite[] spriteSet, bool loop)
        {
            _loop = loop;
            float endFrame = spriteSet.Length;
            _animationInterval = new WaitForSecondsRealtime(1 / endFrame);
            int count = 0;
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

                    if ((1 / endFrame * count) >= _colorChangeWait && spriteSet != _deadSpriteSet)
                    {
                        _isCheckElapsedTime = false;
                        count = 0;
                        spriteSet = _moveSpriteSet;
                        Debug.Log($"시간 초과, 따라서 {_elapsedTime}으로 초기화. 지금 스프라이트셋 {spriteSet}, 체크 여부{_isCheckElapsedTime}");
                    }

                    if (_isCheckElapsedTime && spriteSet != _deadSpriteSet)
                    {
                        count++;
                    }

                    if (_isDamaged == true && spriteSet != _deadSpriteSet)
                    {
                        _isDamaged = false;
                        _isCheckElapsedTime = true;
                        count = 0;
                        spriteSet = _damagedSpriteSet;
                        Debug.Log("대미지 효과 시작");

                    }

                    if (_audioSource != null && _audioClips != null && _audioClips.Length > 0 && i == 0)
                    {
                        int a = Random.Range(0, _audioClips.Length);
                        _audioSource.PlayOneShot(_audioClips[a]);
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
                _spriteRenderer.flipX = _agentBody.Velocity.x <= 0f;
                yield return _waitTime;
            }
        }

        public override void TakeDamage(IDamager damager, float damage)
        {
            _isDamaged = true;
            base.TakeDamage(damager, damage);
        }

        private IEnumerator C_ChangeColor()
        {
            _spriteRenderer.color = COLOR_DAMAGED;
            yield return _colorChangeWait;
            _spriteRenderer.color = _initialColor;
        }

        public override void Dead()
        {
            base.Dead();
            _agentAuthoring.enabled = false;
            _loop = false;

            if (_deadCoroutine == null)
            {
                _deadCoroutine = StartCoroutine(PerformDie());
            }
        }

        private IEnumerator PerformDie()
        {
            //죽음 애니메이션 실행시키기.
            if (_spriteRenderer.enabled)
            {
                yield return StartCoroutine(UpdateSprite(_deadSpriteSet, false));
            }
            DropManager.Instance.DropExp(this.transform, 10f);
            DropManager.Instance.DropItem(this.transform);
            // 풀에 되돌리기.
            OnRelease?.Invoke(this);
            yield break;
        }

        public void OnVolumeChanged(float volume)
        {
            _audioSource.volume = volume;
        }
    }
}
