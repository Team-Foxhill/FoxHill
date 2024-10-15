using System;

namespace FoxHill.Monster.AI
{
    /// <summary>
    /// 중간 분기처럼 사용되는 노드.
    /// 조건을 확인하고, 결과에 따라 하위 노드의 실행 여부를 결정.
    /// </summary>
    public class Decorator : Node, IParentOfChild
    {
        /// <summary>
        /// 하위 노드의 실행 조건을 정의할 때, var checkCombo = new Decorator(monsterBehaviorTree, () => {하위 노드의 실행 조건에 대한 bool 값 반환하도록 설정}와 같이 사용하면 됨.
        /// </summary>
        /// <param name="tree">이 행동을 가지고 있을 트리.</param>
        /// <param name="condition">람다 함수를 활용하여 확인하고자 하는 조건 함수를 작성.</param>
        public Decorator(MonsterBehaviourTree tree, SouthBossMonsterController controller, Func<bool> condition) : base(tree, controller)
        {
            _condition = condition;
            _controller = controller;
        }

        public Node child { get; set; } // _condition의 결과가 true일 때 실행될 노드.
        private Func<bool> _condition; // 실제로 확인할 조건이 담긴 함수를 저장하는 필드.
        private SouthBossMonsterController _controller;

        public override Result Invoke()
        {
            if (_condition.Invoke()) // 조건 함수가 성공을 반환할 경우.
            {
                tree.stack.Push(child); // 실행 스택에 자식 노드를 추가.
                return Result.Success;
            }

            return Result.Failure;
        }

        public void Attach(Node child)
        {
            this.child = child;
        }
    }
}
