using FoxHill.Core.Pause;
using UnityEngine;

namespace FoxHill.Controller
{
    public abstract class CharacterControllerBase : MonoBehaviour, IPausable
    {
        protected bool _isPaused = false;
        protected bool _isDead = false;

        protected virtual void Awake()
        {
            PauseManager.Register(this);
        }

        protected virtual void OnDestroy()
        {
            PauseManager.Unregister(this);
        }

        public virtual void Pause()
        {
            _isPaused = true;
        }

        public virtual void Resume()
        {
            _isPaused = false;
        }
    }
}