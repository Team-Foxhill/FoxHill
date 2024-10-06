using FoxHill.Core.Damage;
using FoxHill.Core.Utils;
using System.Collections;
using UnityEngine;

namespace FoxHill.Player.State.Implementations
{
    public class PlayerAttackState : PlayerStateBase
    {
        public override PlayerState State { get; protected set; } = PlayerState.Attack;
        public override bool IsMoveState { get; protected set; } = false;
        private readonly WaitForEndOfFrame FRAME_END_WAIT = new WaitForEndOfFrame();

        private LayerMask _attackableLayer;
        private float _attackOffset = 1f;
        private Vector2 _attackRange = Vector2.one * 2f;
        private Vector3 _attackPoint;


        protected override void Awake()
        {
            base.Awake();

            _attackableLayer = (1 << LayerRepository.LAYER_PATH_BOSS_MONSTER | 1 << LayerRepository.LAYER_PATH_FOLLOW_MONSTER);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_manager.IsLeftward == true)
            {
                _animator.FlipSprite(true);
            }

            Attack();
        }

        protected override void OnDisable()
        {
            _animator.FlipSprite(false);
        }

        private void Attack()
        {
            StartCoroutine(C_Attack());

            _attackPoint
                = (_manager.IsLeftward == true)
                ? _manager.Transform.position + Vector3.left * _attackOffset
                : _manager.transform.position + Vector3.right * _attackOffset;
        }

        private IEnumerator C_Attack()
        {
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

            yield return FRAME_END_WAIT;

            IsDone = true;
        }

        private void PerformDamage()
        {
            var hits = Physics2D.OverlapBoxAll(_attackPoint, _attackRange, 0f, _attackableLayer);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageable>(out var damageable) == true)
                {
                    damageable.TakeDamage(_manager, _manager.Stat.Power);
                }
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(_attackPoint, _attackRange);
        }
    }
}
