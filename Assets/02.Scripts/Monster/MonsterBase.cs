using UnityEngine;
using FoxHill.Core.Stat;
using FoxHill.Core.Damage;
using IPoolable = FoxHill.Core.IPoolable;
using System;
using FoxHill.Core.Pause;

namespace FoxHill.Monster
{
    public abstract class MonsterBase : MonoBehaviour, IStat, IDamageable, IPausable
    {
        public float MaxHp { get; private set; }
        public float CurrentHp { get; protected set; }
        public float MoveSpeed { get; private set; }
        public float Power { get; private set; }
        public float Defense { get; private set; }


        public event Action OnDead;
        protected int _monsterNumber;
        protected MonsterForm _monsterForm;
        protected bool _isPaused = false;


        /// <summary>
        /// 스탯을 파일에서 불러오는 메서드.
        /// </summary>
        protected virtual void SetStat()
        {
            MaxHp = _monsterForm.MaxHp;
            CurrentHp = MaxHp;
            MoveSpeed = _monsterForm.MoveSpeed;
            Power = _monsterForm.Power;
            Defense = _monsterForm.Defense;
        }

        /// <summary>
        /// 대미지를 처리하는 메서드.
        /// </summary>
        /// <param name="damage">PerformAttack에서 받은 대미지</param>
        public void TakeDamage(float damage)
        {
            //프로퍼티 체력 깎기.
            CurrentHp -= damage;
            if (CurrentHp <= 0f) Dead();
        }

        /// <summary>
        /// 죽음을 처리하는 메서드.
        /// </summary>
        public virtual void Dead()
        {
            OnDead?.Invoke();
        }

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