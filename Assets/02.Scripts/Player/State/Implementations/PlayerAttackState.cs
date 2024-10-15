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

        private LayerMask _attackableLayer;
        private float _attackOffset = 1f;
        private Vector2 _attackRange = Vector2.one * 2f;
        private Vector3 _attackPoint;


        protected override void Awake()
        {
            base.Awake();

            _attackableLayer = (1 << LayerRepository.LAYER_BOSS_MONSTER | 1 << LayerRepository.LAYER_PATH_FOLLOW_MONSTER);
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

            foreach ( var hit in hits ) 
            {
                if (hit.TryGetComponent<IStaggerable>(out var staggerable) == true)
                {               
                    if(staggerable.IsFatalAttackable == true)
                    {
                        if (hit.TryGetComponent<IDamageable>(out var damageable) == true)
                        {

                            _manager.State.Parameters.FatalAttackTarget = damageable;
                            _manager.SetState(PlayerState.FatalAttack, true);

                            IsDone = true;
                            return;
                        }
                    }
                }
            }

            StartCoroutine(C_Attack());
        }

        //private bool IsFatalAttackableRange(Transform target)
        //{
        //    if(Vector3.Angle(target.position, _manager.transform.position) < 60f)
        //    {

        //    }
        //}

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