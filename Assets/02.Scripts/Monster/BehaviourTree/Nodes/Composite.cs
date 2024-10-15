using System.Collections.Generic;

namespace FoxHill.Monster.AI
{
    public abstract class Composite : Node, IParentOfChilldren
    {
        public Composite(MonsterBehaviourTree behaviourTree, SouthBossMonsterController controller) : base(behaviourTree, controller)
        {
            children = new List<Node>();
            _controller = controller;
        }

        public List<Node> children { get; set; }
        protected int currentChildIndex; // 현재 실행 중인 자식 노드의 인덱스. 실행 상태를 유지하고 중단된 지점에서 재개하는 데 사용.
        private SouthBossMonsterController _controller;


        public void Attach(Node child)
        {
            children.Add(child);
        }
    }
}
