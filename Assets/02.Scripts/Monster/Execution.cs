using System;

namespace FoxHill.Monster.AI
{
    /// <summary>
    /// AI의 구체적인 행동을 실행하는 클래스.
    /// 행동 트리의 잎 노드로 사용되며, AI의 행동을 캡슐화함.
    /// </summary>
    public class Execution : Node
    {
        /// <summary>
        /// AI의 새로운 행동을 정의할 때, var attackAction = new Execution(monsterBehaviorTree, () => {각 조건과, 각 Result 반환하도록 설정}와 같이 사용하면 됨.
        /// </summary>
        /// <param name="tree">이 행동을 가지고 있을 트리.</param>
        /// <param name="execute">람다식을 활용하여 실행될 함수와 경우에 따라 돌려줄 값을 정의.</param>
        public Execution(MonsterBehaviourTree tree, Func<Result> execute) : base(tree)
        {
            _execute = execute;
        }

        private Func<Result> _execute; // 실제로 실행될 행동이 담긴 함수를 저장하는 필드.

        /// <summary>
        /// 실행 상황에 대해 반환하는 메서드.
        /// </summary>
        /// <returns></returns>
        public override Result Invoke()
        {
            return _execute.Invoke();
        }
    }
}
