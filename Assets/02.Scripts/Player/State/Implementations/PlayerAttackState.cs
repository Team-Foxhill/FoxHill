using FoxHill.Core.Damage;
using FoxHill.Core.Utils;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace FoxHill.Player.State.Implementations
{
    public class PlayerAttackState : PlayerStateBase
    {
        [SerializeField] AudioClip[] _attackNonTargetSounds;
        [SerializeField] AudioClip[] _attackTargetSounds;
        public override PlayerState State { get; protected set; } = PlayerState.Attack;
        public override bool IsMoveState { get; protected set; } = false;

        private const float FATAL_ATTACKABLE_RANGE = 30f;

        private LayerMask _attackableLayer;
        private float _attackOffset = 1f;
        private Vector2 _attackRange = Vector2.one * 3f;
        private Vector3 _attackPoint;
        private bool _isPlayedOnce;


        protected override void Awake()
        {
            base.Awake();

            _attackableLayer = (1 << LayerRepository.LAYER_BOSS_MONSTER | 1 << LayerRepository.LAYER_PATH_FOLLOW_MONSTER);
        }

        protected override void OnEnable()
        {
            IsDone = false;

            if (_manager.IsLeftward == true)
            {
                _animator.FlipSprite(true);
            }

            Attack();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _animator.FlipSprite(false);
        }

        private void Attack()
        {
            _attackPoint
                = (_manager.IsLeftward == true)
                ? _manager.Transform.position + Vector3.left * _attackOffset
                : _manager.transform.position + Vector3.right * _attackOffset;

            //// 보스와 일반몬스터가 동시에 존재할 수 있거나 여러 보스가 있을 수 있다면 수정 필요
            //// 특수공격(패링 후 반격) 판단
            var hits = Physics2D.OverlapBoxAll(_attackPoint, _attackRange, 0f, 1 << LayerRepository.LAYER_BOSS_MONSTER);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IStaggerable>(out var staggerable) == true)
                {
                    if (staggerable.IsFatalAttackable == true)
                    {
                        if ((hit.TryGetComponent<IDamageable>(out var damageable) == true)
                            && (IsFatalAttackableRange(hit.transform) == true))
                        {
                            _manager.State.Parameters.FatalAttackTarget = damageable;
                            _manager.State.Parameters.StaggerTarget = staggerable;
                            _manager.SetState(PlayerState.FatalAttack, true);

                            return;
                        }
                    }
                }
            }

            StartCoroutine(C_Attack());
        }

        private bool IsFatalAttackableRange(Transform target)
        {
            var angle = Vector3.Angle((target.position - _manager.transform.position).normalized, Vector2.right);
            angle = (angle > 90f) ? 180f - angle : angle;

            return (angle < FATAL_ATTACKABLE_RANGE);
        }

        private IEnumerator C_Attack()
        {
            PlayAnimation();

            bool isPerformed = false;

            yield return null;

            while (_animator.AnimationTime < 1f)
            {
                if (_animator.AnimationTime > 0.6f && isPerformed == false)
                {
                    isPerformed = true;
                    PerformDamage();
                }

                yield return null;
            }

            if (_isPlayedOnce == true && _animator.AnimationTime > 0.9f)
            {
                _isPlayedOnce = false;
            }

            yield return FRAME_END_WAIT;

            IsDone = true;
        }

        private void PerformDamage()
        {
            var hits = Physics2D.OverlapBoxAll(_attackPoint, _attackRange, 0f, _attackableLayer);
            if (hits.Length == 0 && _audioSource.isPlaying == false)
            {
                int i = (int)Random.Range(0, _attackNonTargetSounds.Length);
                _audioSource.PlayOneShot(_attackNonTargetSounds[i]);
            }

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageable>(out var damageable) == true)
                {
                    if (_isPlayedOnce == false)
                    {
                        _isPlayedOnce = true;
                int i = (int)Random.Range(0, _attackTargetSounds.Length);
                        _audioSource.PlayOneShot(_attackTargetSounds[i]); // 공격 성공 사운드.
                    }

                    damageable.OnDead += OnKillMonster;

                    damageable.TakeDamage(_manager, _manager.Stat.Power);

                    if (damageable != null)
                        damageable.OnDead -= OnKillMonster;
                }
            }
        }

        private void OnKillMonster()
        {
            _manager.OnKillMonster.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(_attackPoint, _attackRange);
        }
    }
}
