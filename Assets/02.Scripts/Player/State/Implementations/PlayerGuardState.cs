using System.Collections;
using UnityEngine;

namespace FoxHill.Player.State.Implementations
{
    public class PlayerGuardState : PlayerStateBase
    {
        public override PlayerState State { get; protected set; } = PlayerState.Guard;
        public override bool IsMoveState { get; protected set; } = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(C_Guard());
        }

        private IEnumerator C_Guard()
        {
            while(_animator.AnimationTime < 1f)
            {
                yield return null;
            }

            IsDone = true;
        }

        public void PerfectGuard()
        {
            _manager.IsPerfectGuard = !_manager.IsPerfectGuard;
        }
    }
}