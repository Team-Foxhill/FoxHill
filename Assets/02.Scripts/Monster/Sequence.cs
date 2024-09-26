using System.Collections;

namespace FoxHill.Monster.AI
{
    /// <summary>
    /// 자식 노드들을 순차적으로 실행하며, 실패하거나 실행 중인 첫 번째 자식의 결과를 반환하는 복합 노드.
    /// 모든 자식이 성공한 경우에만 성공을 반환한다.
    /// </summary>
    public class Sequence : Composite
    {
        public Sequence(MonsterBehaviourTree tree) : base(tree)
        {
        }

        public override Result Invoke()
        {
            Result result = Result.Success;
            //이전에 Running 상태로 중단된 지점부터 실행을 재개하기 위해 currentChildIndex를 사용.
            for (int i = currentChildIndex; i < children.Count; i++)
            {
                result = children[i].Invoke();
                switch (result)
                {
                    // 성공 값이 반환되면 다음 대상을 탐색.
                    case Result.Success:
                        {
                            currentChildIndex++;
                        }
                        break;
                    // 실패하면 멈추고 다시 처음부터 탐색.
                    case Result.Failure:
                        {
                            currentChildIndex = 0;
                            return result;
                        }
                    // 진행중일 경우, 대기를 이어갈 수 있도록 값을 그대로 반환.
                    case Result.Running:
                        {
                            return result;
                        }
                    default:
                        throw new System.Exception($"Invalid result code : {result}");
                }
            }

            currentChildIndex = 0;
            return result;
        }
    }
}
