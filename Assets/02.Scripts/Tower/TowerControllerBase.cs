using UnityEngine;
using FoxHill.Core.Stat;
using FoxHill.Core.Damage;
using System.Collections;
using System.Collections.Generic;
using System;
using FoxHill.Core.Pause;

namespace FoxHill.Tower
{
    public abstract class TowerControllerBase : MonoBehaviour, IStat, IDamageable, IPausable
    {
        public Color gizmoColor = new Color(1, 0, 0, 0.2f);
        public event Action OnDead;
        public float MaxHp { get; private set; }
        public float CurrentHp { get; protected set; }
        public float AttackSpeed { get; private set; }
        public float AttackRange { get; private set; }
        public float MoveSpeed { get => _moveSpeed; private set => _moveSpeed = 0f; }
        public float Power { get; private set; }
        public float Defense { get; private set; }
        [SerializeField] protected int _towerIndex;
        [SerializeField] protected LayerMask _targetLayer;
        [SerializeField] protected GameObject _bullet;
        private float _moveSpeed = 0f;
        private bool _isDamageable = false;
        protected float _attackInterval;
        protected TowerForm _towerForm;
        protected HashSet<Collider2D> objectsInTrigger = new HashSet<Collider2D>();
        protected Collider2D _attackTarget;
        protected IDamageable _damageable;
        protected bool _isPaused = false;


        protected virtual void Start()
        {
            TowerDataManager.TryGetTower(_towerIndex, out _towerForm);
            SetStat();
            CircleCollider2D collider = gameObject.GetComponent<CircleCollider2D>();
            collider.radius = AttackRange;
            _attackInterval = (1 / AttackSpeed);
            StartCoroutine(PerformTowerFunction());
        }

        /// <summary>
        /// 스탯을 파일에서 불러오는 메서드.
        /// </summary>
        private void SetStat()
        {
            MaxHp = _towerForm.MaxHp;
            CurrentHp = MaxHp;
            AttackSpeed = _towerForm.AttackSpeed;
            AttackRange = _towerForm.AttackRange;
            Power = _towerForm.Power;
            Defense = _towerForm.Defense;
        }


        /// <summary>
        /// 대미지를 처리하는 메서드.
        /// </summary>
        /// <param name="damage">PerformAttack에서 받은 대미지</param>
        public void TakeDamage(float damage)
        {
            if (_isDamageable == true)
            {
                //프로퍼티 체력 깎기.
                CurrentHp -= damage;
                if (CurrentHp <= 0f) Dead();
            }
        }

        /// <summary>
        /// 죽음을 처리하는 메서드.
        /// </summary>
        public void Dead()
        {
            OnDead.Invoke();
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & _targetLayer) != 0)
            {
                objectsInTrigger.Add(collision);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & _targetLayer) != 0)
            {
                objectsInTrigger.Remove(collision);
            }
        }

        /// <summary>
        /// 공격할 때마다 범위 내의 랜덤 대상을 지정하여 공격하는 형태.
        /// 만일 한 대상이 죽을 때까지 공격해야 한다면 그건 조금 생각해볼 필요가 있을 듯.
        /// 여기 부분에 두 좌표(타워, 대상 몬스터) 사이의 직선 경로로 날아가는 타워 발사체 스프라이트 하나 필요.
        /// 나중에 느리지만 랜덤 대상에게 큰 대미지 주는 바리에이션 타워로 써도 될 듯.
        /// </summary>
        /// <returns></returns>
        //private IEnumerator PerformAttack()
        //{
        //    while (true)
        //    {
        //        if (objectsInTrigger.Count == 0)
        //        {
        //            yield return _attackInterval;
        //            continue;
        //        }
        //        Collider2D randomCollider = objectsInTrigger.ElementAt(Random.Range(0, objectsInTrigger.Count));
        //        IDamageable damageable = randomCollider.gameObject.GetComponent<IDamageable>();
        //        if (damageable != null)
        //        {
        //            DebugFox.Log("TowerAttackPerformed!");
        //            damageable.TakeDamage(Power);
        //        }
        //        yield return _attackInterval;
        //    }
        //}

        /// <summary>
        /// 범위 내의 랜덤 대상이 죽거나 해시셋에서 사라질 때까지 공격하는 형태.

        /// 여기 부분에 두 좌표(타워, 대상 몬스터) 사이의 직선 경로로 날아가는 타워 발사체 스프라이트 하나 필요.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator PerformTowerFunction();

        public void Pause()
        {
            _isPaused = true;

        }

        public void Resume()
        {
            _isPaused = false;
        }
    }
}