using UnityEngine;

namespace FoxHill.Player.State
{
    public abstract class BehaviourBase : StateMachineBehaviour
    {
        protected PlayerManager _manager;

        public void Initialize(PlayerManager manager)
        {
            _manager = manager;
        }
    }
}