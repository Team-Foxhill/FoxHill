using UnityEngine;

namespace FoxHill.Monster.AI
{
    public class MonsterBehaviourTree : MonoBehaviour, IBuilder
    {
        public Blackboard blackboard { get; private set; }

        public MonsterBehaviourTree build()
        {
            throw new System.NotImplementedException();
        }

        public IBuilder Decorator()
        {
            throw new System.NotImplementedException();
        }

        public IBuilder Execution()
        {
            throw new System.NotImplementedException();
        }

        public IBuilder FinishCurrentComposite()
        {
            throw new System.NotImplementedException();
        }

        public IBuilder Parallel()
        {
            throw new System.NotImplementedException();
        }

        public IBuilder Seek()
        {
            throw new System.NotImplementedException();
        }

        public IBuilder Selector()
        {
            throw new System.NotImplementedException();
        }

        public IBuilder Sequence()
        {
            throw new System.NotImplementedException();
        }
    }
}