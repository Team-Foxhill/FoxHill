using UnityEngine;
using FoxHill.Core;
using FoxHill.Core.Stat;
using FoxHill.Core.Damage;

namespace FoxHill.Monster
{
    public abstract class MonsterBase : MonoBehaviour, IStat, IDamageable
    {
        public float MaxHp { get; private set; }
        public float CurrentHp { get; protected set; }
        public float MoveSpeed { get; private set; }
        public float Power { get; private set; }
        public float Defense { get; private set; }


        protected int _monsterNumber;
        protected MonsterForm _monsterForm;


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

        protected virtual void PerformMove()
        {

        }

        /// <summary>
        /// 대미지를 처리하는 메서드.
        /// </summary>
        /// <param name="damage">PerformAttack에서 받은 대미지</param>
        public abstract void TakeDamage(float damage);

        /// <summary>
        /// 죽음을 처리하는 메서드.
        /// </summary>
        public abstract void Dead();

    }
}