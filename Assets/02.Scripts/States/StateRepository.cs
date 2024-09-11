using UnityEngine;

namespace FoxHill.States
{
    public enum State
    {
        Idle,
        Move,
        Attack,
        Dodge,
        Skill,
        Dead
    }

    public static class StateRepository
    {
        public static readonly int HASH_ID_STATE = Animator.StringToHash("State");
        public static readonly int HASH_ID_IS_DIRTY = Animator.StringToHash("IsDirty");
    }
}