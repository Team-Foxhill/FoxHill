using UnityEngine;

namespace FoxHill.States.Behaviour
{
    public abstract class BehaviourBase : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetBool(StateRepository.HASH_ID_IS_DIRTY, false);
        }
    }
}