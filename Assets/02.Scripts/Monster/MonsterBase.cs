using UnityEngine;
using FoxHill.Core;
using FoxHill.Core.Stat;
using FoxHill.Core.Damage;

namespace FoxHill.Monster
{
    public abstract class MonsterBase : MonoBehaviour, IStat, IDamageable
    {
        public float MaxHp { get; protected set; }
        public float CurrentHp { get; protected set; }
        public float MoveSpeed { get; protected set; }
        public float Power { get; protected set; }
        public float Defense { get; protected set; }


    protected int _monsterNumber;


        /// <summary>
        /// 스탯을 파일에서 불러오는 메서드.
        /// </summary>
        protected abstract void SetStat();

        protected virtual void PerformMove()
        {
            
        }

        /// <summary>
        /// 공격을 실행하는 메서드.
        /// </summary>
        /// <returns>대미지 양.</returns>

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