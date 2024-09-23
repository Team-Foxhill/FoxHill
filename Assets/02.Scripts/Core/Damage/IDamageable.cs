using System;

namespace FoxHill.Core.Damage
{
    public interface IDamageable
    {
        event Action OnDead;
        void TakeDamage(float damage);
        void Dead();
    }
}