namespace FoxHill.Monster.AI
{
    public class Parallel : Composite
    {
        /// <summary>
        /// 성공 조건 정리하기.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="successCountRequired"></param>
        public Parallel(MonsterBehaviourTree tree, int successCountRequired) : base(tree)
        {
            _successCountRequired = successCountRequired;
        }

        private int _successCountRequired; // 성공 정책. 이 개수 이상 성공해야 성공 반환.
        private int _successCount;

        public override Result Invoke()
        {
            Result result = Result.Failure; // 기본 반환값.

            for (int i = currentChildIndex; i < children.Count; i++)
            {
                result = children[i].Invoke();
                switch (result)
                {
                    case Result.Success:
                        {
                            _successCount++; // 자식에게서 성공을 반환받을 때마다 _successCount에 1씩 더함.
                        }
                        break;
                    case Result.Failure:
                        {
                        }
                        break;
                    case Result.Running:
                        {
                            return result;
                        }
                        default:
                        throw new System.Exception($"Invalid result code : {result}");
                }
            }

            result = _successCount >= _successCountRequired ? Result.Success : Result.Failure; // 성공 정책과 비교하여 결과 반환.
            _successCount = 0; // 센 숫자 초기화.
            currentChildIndex = 0; // 현재 선택된 자식 번호 초기화.
            return result;
        }
    }
}
