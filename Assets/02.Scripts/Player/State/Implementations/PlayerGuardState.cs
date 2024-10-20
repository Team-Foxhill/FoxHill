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
            while (_animator.AnimationTime < 1f)
            {
                yield return null;
            }

            IsDone = true;
        }

        public void PerfectGuard()
        {
            StartCoroutine(C_PerfectGuard());
        }

        private IEnumerator C_PerfectGuard()
        {
            while (_animator.AnimationTime < 0.5f && State == PlayerState.Guard)
            {
                _manager.IsPerfectGuard = true;
                yield return null;
            }
            _manager.IsPerfectGuard = false;
            yield break;
        }
    }
}