using UnityEngine;

namespace FoxHill.Player.State
{
    public abstract class PlayerStateBase : MonoBehaviour
    {
        public PlayerStateBase State => _state;
        protected abstract PlayerStateBase _state { get; set; }

        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}