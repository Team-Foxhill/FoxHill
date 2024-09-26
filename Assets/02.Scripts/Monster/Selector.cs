namespace FoxHill.Monster.AI
{
    /// <summary>
    /// 자식 노드들을 순차적으로 실행하며, 성공하거나 실행 중인 첫 번째 자식의 결과를 반환하는 복합 노드.
    /// 모든 자식이 실패할 경우에만 실패를 반환한다.
    /// </summary>
    public class Selector : Composite
    {
        public Selector(MonsterBehaviourTree tree) : base(tree)
        {
        }

        public override Result Invoke()
        {
            Result result = Result.Failure;
            for (int i = currentChildIndex; i < children.Count; i++)
            {
                result = children[i].Invoke();

                switch (result)
                {
                    // 성공하면 값을 반환.
                    case Result.Success:
                        {
                            currentChildIndex = 0;
                            return result;
                        }
                        
                    // 실패 값이 반환되면 다음 대상을 탐색.
                    case Result.Failure:
                        {
                            currentChildIndex++;
                        }
                            break;
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
