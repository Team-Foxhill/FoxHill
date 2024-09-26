using FoxHill.Core.Damage;
using FoxHill.Core.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Tower
{
    [RequireComponent(typeof(CircleCollider2D))] // 공격 범위 트리거
    public abstract class TowerAttackControllerBase : MonoBehaviour
    {
        protected CircleCollider2D _attackTrigger;
        protected int _targetLayer;

        protected HashSet<IDamageable> _objectsInTrigger = new HashSet<IDamageable>();
        protected IDamageable _attackTarget;

        [SerializeField] protected GameObject _bullet;
        protected SpriteRenderer _bulletRenderer;

        protected float _attackInterval;
        protected WaitForSecondsRealtime _attackIntervalWait;

        protected bool _isPaused = true;
        protected Vector3 _startPosition;

        protected float _attackDamage;

        public void Initialize(float attackRange, float attackSpeed, float attackDamage)
        {
            _attackTrigger = GetComponent<CircleCollider2D>();
            _targetLayer = LayerRepository.LAYER_PATH_FOLLOW_MONSTER;
            _bulletRenderer = _bullet.GetComponent<SpriteRenderer>();

            _attackTrigger.radius = attackRange;

            _attackInterval = (1 / attackSpeed);
            _attackDamage = attackDamage;
            _attackIntervalWait = new WaitForSecondsRealtime(_attackInterval);

            _startPosition = transform.position;
        }

        public abstract void StartAttack();

        public void Pause()
        {
            _isPaused = true;
        }
        public void Resume()
        {
            _isPaused = false;
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == _targetLayer)
            {
                if (collision.TryGetComponent<IDamageable>(out var damageable) == true)
                {
                    _objectsInTrigger.Add(damageable);
                }
            }
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == _targetLayer)
            {
                if (collision.TryGetComponent<IDamageable>(out var damageable) == true)
                {
                    _objectsInTrigger.Remove(damageable);
                }
            }
        }
    }
}