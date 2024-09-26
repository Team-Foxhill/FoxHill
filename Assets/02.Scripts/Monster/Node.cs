using Unity.VisualScripting;

namespace FoxHill.Monster.AI
{
    /// <summary>
    /// 노드의 기반 클래스.
    /// </summary>
    public abstract class Node
    {
        public Node(MonsterBehaviourTree tree)
        {
            this.tree = tree; // 노드의 트리를 몬스터행동트리 클래스에 자동으로 연결.
            this.blackboard = tree.blackboard; // 노드가 결과를 내보낼 블랙보드를 트리를 통해 연결.
        }

        protected MonsterBehaviourTree tree;
        protected Blackboard blackboard;

        /// <summary>
        /// 탐색 결과를 내보내주는 메서드.
        /// </summary>
        /// <returns></returns>
        public abstract Result Invoke();
    }
}
