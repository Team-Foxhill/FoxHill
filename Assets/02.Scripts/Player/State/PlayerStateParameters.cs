using FoxHill.Core.Damage;
using UnityEngine;

namespace FoxHill.Player.State
{
    public class PlayerStateParameters : ScriptableObject
    {
        public Transform AttackerTransform;
        public IDamageable FatalAttackTarget;
        public IStaggerable StaggerTarget;
    }
}
