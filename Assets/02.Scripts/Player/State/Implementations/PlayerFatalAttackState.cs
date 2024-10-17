using FoxHill.Core.Damage;
using System.Collections;

namespace FoxHill.Player.State.Implementations
{
    public class PlayerFatalAttackState : PlayerStateBase
    {
        public override PlayerState State { get; protected set; } = PlayerState.FatalAttack;
        public override bool IsMoveState { get; protected set; } = false;

        private float _damageMultiplier = 2f;

        private IDamageable _attackTarget;
        private IStaggerable _staggerTarget;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_manager.IsLeftward == true)
            {
                _animator.FlipSprite(true);
            }

            StartCoroutine(C_FatalAttack());
        }

        protected override void OnDisable()
        {
            _animator.FlipSprite(false);
        }

        public override void Initialize(PlayerStateParameters parameters)
        {
            _attackTarget = parameters.FatalAttackTarget;
        }

        private IEnumerator C_FatalAttack()
        {
            _attackTarget?.TakeDamage(_manager, _manager.Stat.Power * _damageMultiplier);
            _staggerTarget?.MakeStun(_manager);

            while (_animator.AnimationTime < 1f)
            {
                yield return null;
            }

            yield return FRAME_END_WAIT;

            IsDone = true;
        }
    }
}