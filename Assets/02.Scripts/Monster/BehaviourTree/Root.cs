using Unity.VisualScripting;

namespace FoxHill.Monster.AI
{
    /// <summary>
    /// 최상위 노드. 이 노드부터 순회를 시작함.
    /// </summary>
    public class Root : Node, IParentOfChild
    {

        public Root(MonsterBehaviourTree tree, SouthBossMonsterController controller) : base(tree, controller) // 부모 클래스(Node)의 생성자를 호출하여 트리 참조를 초기화.
        {
            _controller = controller;
        }

        public Node child { get; set; }
        private SouthBossMonsterController _controller;

        public void Attach(Node child)
        {
            this.child = child;
        }

        public override Result Invoke()
        {
            tree.stack.Push(child);
            return Result.Success;
        }
    }
}
