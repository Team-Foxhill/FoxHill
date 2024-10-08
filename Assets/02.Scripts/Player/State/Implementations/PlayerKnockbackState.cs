using System.Collections;
using UnityEngine;

namespace FoxHill.Player.State.Implementations
{
    public class PlayerKnockbackState : PlayerStateBase
    {
        public override PlayerState State { get; protected set; } = PlayerState.Knockback;
        public override bool IsMoveState { get; protected set; } = false;

        private const float KNOCKBACK_SPEED = 0.5f;
        private const float KNOCKBACK_TIME = 0.8f;

        private Vector2 _direction;
        private Transform _playerTransform;

        protected override void OnEnable()
        {
            base.OnEnable();

            _playerTransform = _manager.Transform;

            StartCoroutine(C_Knockback());
        }

        public override void Initialize(PlayerStateParameters parameters)
        {
            _direction = (_manager.Transform.position - parameters.AttackerTransform.position).normalized;
        }

        private IEnumerator C_Knockback()
        {
            float elapsedTime = 0f;

            while (elapsedTime < KNOCKBACK_TIME)
            {
                Vector2 knockbackPosition = _direction * KNOCKBACK_SPEED * Time.deltaTime;

                _playerTransform.Translate(knockbackPosition);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            IsDone = true;
        }
    }
}