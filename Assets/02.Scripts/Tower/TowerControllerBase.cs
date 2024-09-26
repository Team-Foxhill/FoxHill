using FoxHill.Core;
using FoxHill.Core.Damage;
using FoxHill.Core.Pause;
using FoxHill.Core.Stat;
using System;
using System.Collections;
using UnityEngine;

namespace FoxHill.Tower
{
    [RequireComponent(typeof(CircleCollider2D))] // 타워 자체의 크기 트리거
    public abstract class TowerControllerBase : MonoBehaviour, IDamageable, IPausable
    {
        public event Action OnDead;
        public int Index => _towerIndex;
        public Transform Transform => gameObject.transform;

        protected class TowerStat : IStat
        {
            public TowerStat(TowerForm form)
            {
                MaxHp = form.MaxHp;
                CurrentHp = MaxHp;
                AttackSpeed = form.AttackSpeed;
                AttackRange = form.AttackRange;
                MoveSpeed = 0f;
                Power = form.Power;
                Defense = form.Defense;
            }

            public float MaxHp { get; set; }
            public float CurrentHp { get; set; }
            public float AttackSpeed { get; set; }
            public float AttackRange { get; set; }
            public float MoveSpeed { get; set; }
            public float Power { get; set; }
            public float Defense { get; set; }
        }

        protected TowerStat _stat;
        protected CircleCollider2D _trigger;

        [SerializeField] protected int _towerIndex;

        protected bool _isDamageable = false;
        protected bool _isPaused = false;


        protected virtual void Awake()
        {
            _trigger = GetComponent<CircleCollider2D>();

            if (TowerDataManager.TryGetTower(_towerIndex, out var towerForm) == true)
            {
                _stat = new TowerStat(towerForm);

                if (towerForm.TowerType == TowerType.DefenseTower)
                {
                    _isDamageable = true;
                }
            }
            else
            {
                DebugFox.LogError("Failed to initialize TowerStat");
            }

            PauseManager.Register(this);
        }

        protected virtual void Start()
        {
            StartCoroutine(PerformTowerFunction());
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
                _stat.CurrentHp -= damage;

                if (_stat.CurrentHp <= 0f)
                {
                    Dead();
                }
            }
        }

        /// <summary>
        /// 죽음을 처리하는 메서드.
        /// </summary>
        public void Dead()
        {
            OnDead.Invoke();
            PauseManager.Unregister(this);
            Destroy(gameObject);
        }

        public virtual void Pause()
        {
            _isPaused = true;
        }

        public virtual void Resume()
        {
            _isPaused = false;
        }

        protected abstract IEnumerator PerformTowerFunction();


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

    }
}