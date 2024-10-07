using System.Security.Cryptography;
using UnityEngine;

namespace FoxHill.Player.State
{
    [RequireComponent(typeof(PlayerAnimationController))]
    public abstract class PlayerStateBase : MonoBehaviour
    {
        public abstract PlayerState State { get; protected set; }
        public abstract bool IsMoveState { get; protected set; }
        public bool IsDone { get; protected set; } = false; // State의 상태가 끝났음을 StateManager에 알리는 프로퍼티

        protected PlayerManager _manager;
        protected PlayerAnimationController _animator;

        protected readonly WaitForEndOfFrame FRAME_END_WAIT = new WaitForEndOfFrame();

        protected virtual void Awake()
        {
            _manager = GetComponentInParent<PlayerManager>();
            _animator = GetComponent<PlayerAnimationController>();

            this.enabled = false;
        }

        protected virtual void OnEnable()
        {
            IsDone = false;
            PlayAnimation();
        }

        protected virtual void Update()
        {

        }

        protected virtual void OnDisable()
        {
        }

        public virtual void Initialize(PlayerStateParameters parameters)
        {

        }

        public void PlayAnimation()
        {
            _animator.PlayAnimation(State);
        }
    }
}