using UnityEngine;

namespace FoxHill.Core.Knockback
{
    public interface IKnockbackable
    {
        public void Knockback(Transform attackerTransform);
    }
}