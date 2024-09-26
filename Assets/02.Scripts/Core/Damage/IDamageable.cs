using System;
using UnityEngine;

namespace FoxHill.Core.Damage
{
    public interface IDamageable
    {
        Transform Transform { get; }
        event Action OnDead;
        void TakeDamage(float damage);
        void Dead();
    }
}