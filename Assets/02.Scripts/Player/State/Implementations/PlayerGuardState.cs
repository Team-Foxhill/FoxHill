using System.Collections;
using UnityEngine;

namespace FoxHill.Player.State.Implementations
{
    public class PlayerGuardState : PlayerStateBase
    {
        private const float GUARD_TIME = 1f;

        public override PlayerState State { get; protected set; } = PlayerState.Guard;
        public override bool IsMoveState { get; protected set; } = false;

        private bool _isAttacked = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            _isAttacked = false;

            StartCoroutine(C_Guard());
        }

        private IEnumerator C_Guard()
        {
            float elapsedTime = 0f;

            while (elapsedTime < GUARD_TIME)
            {
                if(_isAttacked == true) // 퍼펙트 가드 성공
                {
                    PerfectGuard();
                    yield break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            IsDone = true;
        }

        private void PerfectGuard()
        {
            // TODO : 보스에게 그로기 신호
        }
    }
}