using System.Collections;

namespace FoxHill.Monster.AI
{
    /// <summary>
    /// 자식 노드들을 순차적으로 실행하는 컴포지트 노드.
    /// 하나의 자식이 실패하면 전체가 실패하고 처음부터 다시 시작.
    /// 자식이 진행 중(Running)이면 그 상태를 유지.
    /// 모든 자식이 성공한 경우에만 전체가 성공.
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
