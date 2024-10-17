using Unity.VisualScripting;

namespace FoxHill.Monster.AI
{
    /// <summary>
    /// 최상위 노드. 이 노드부터 순회를 시작함.
    /// </summary>
    public class Root : Node, IParentOfChild
    {

        public Root(MonsterBehaviourTree tree) : base(tree) // 부모 클래스(Node)의 생성자를 호출하여 트리 참조를 초기화.
        {
        }

        public Node child { get; set; }

        public void Attach(Node child)
        {
            this.child = child;
        }

        public override Result Invoke()
        {
            tree.NodeStack.Push(child);
            return Result.Success;
        }
    }
}
