using System.Collections;
using UnityEngine;

namespace FoxHill.Player.State.Implementations
{
    public class PlayerDodgeState : PlayerStateBase
    {
        public override PlayerState State { get; protected set; } = PlayerState.Dodge;
        public override bool IsMoveState { get; protected set; } = false;

        private const float DODGE_SPEED = 3f;
        private const float DODGE_TIME = 0.2f;

        private Transform _playerTransform;
        private Vector2 _direction;
        private MotionTrailGenerator _generator;

        protected override void OnEnable()
        {
            IsDone = false;
            
            _playerTransform = _manager.Transform;
            _direction = _manager.Direction.normalized;
            _generator = gameObject.GetComponent<MotionTrailGenerator>();

            StartCoroutine(C_Dodge());
        }

        private IEnumerator C_Dodge()
        {
            float elapsedTime = 0f;
            _generator.On();

            while (elapsedTime < DODGE_TIME)
            {
                Vector2 movePosition = _direction * _manager.Stat.MoveSpeed * DODGE_SPEED * Time.deltaTime;

                _playerTransform.Translate(movePosition);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            _generator.Off();
            IsDone = true;
        }
    }
}
