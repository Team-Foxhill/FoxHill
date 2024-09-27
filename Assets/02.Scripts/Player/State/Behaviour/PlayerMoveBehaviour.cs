using UnityEngine;

namespace FoxHill.Player.State.Behaviour
{
    public class PlayerMoveBehaviour : BehaviourBase
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Vector2 movePosition = _manager.Direction * _manager.Stat.MoveSpeed * Time.deltaTime;
            _manager.CharacterController.Move(movePosition);
        }
    }
}
