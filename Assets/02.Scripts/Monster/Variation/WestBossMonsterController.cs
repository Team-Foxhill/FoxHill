using UnityEngine;

namespace FoxHill.Monster.AI
{
    public class WestBossMonsterController : MonoBehaviour
    {
        [Header("AI Behaviours")]
        [Header("Seek")]
        [SerializeField] float _seekRadius = 3.0f;
        [SerializeField] float _seekAngle = 80f;
        [SerializeField] int _seektargetLayer = 3;
        [SerializeField] float _seekMaxDistance = 10.0f;
        [SerializeField] float _attackRange = 1.0f;
        [SerializeField] float _specialAttackMinRange = 0.6f;
        [SerializeField] float _specialAttackProbability = 0.3f;
        [SerializeField] Transform _target;

        private void Awake()
        {
            // todo.Villin 체이닝 함수를 활용하여 빌드. 현재는 사용 예시로만 작성.
            MonsterBehaviourTree tree = GetComponent<MonsterBehaviourTree>();
            tree = new MonsterBehaviourTreeBuilder()
             .Selector()
                .Sequence()
                    .Decorator(() => IsTargetInRange(_attackRange))
                    .Selector()
                        .Sequence()
                            .Decorator(() => IsTargetInRange(_specialAttackMinRange, _attackRange) && Random.value < _specialAttackProbability)
                            .Execution(() => PerformSpecialAttack())
                        .FinishCurrentComposite()
                        .Sequence()
                            .Execution(() => PerformAttack1())
                            .Execution(() => PerformAttack2())
                            .Execution(() => PerformAttack3())
                        .FinishCurrentComposite()
                    .FinishCurrentComposite()
                .FinishCurrentComposite()
                .Sequence()
                    .Decorator(() => !IsTargetInRange(_attackRange))
                    .Execution(() => MoveTowardsTarget())
                .FinishCurrentComposite()
            .FinishCurrentComposite()
            .Build();
        }
        private bool IsTargetInRange(float minRange, float maxRange)
        {
            float distance = Vector3.Distance(transform.position, _target.position);
            return distance >= minRange && distance <= maxRange;
        }

        private bool IsTargetInRange(float maxRange)
        {
            return Vector3.Distance(transform.position, _target.position) <= maxRange;
        }


        private Result MoveTowardsTarget()
        {
            // 실제 구현에서는 Agent를 사용. 현재는 임시.
            transform.position = Vector3.MoveTowards(transform.position, _target.position, Time.deltaTime * 5f);
            return Result.Success;
        }

        private Result PerformAttack1()
        {
            Debug.Log("Performing Attack 1");
            // 실제 공격 로직 구현할 것.
            return Result.Success;
        }

        private Result PerformAttack2()
        {
            Debug.Log("Performing Attack 2");
            // 실제 공격 로직 구현할 것.
            return Result.Success;
        }

        private Result PerformAttack3()
        {
            Debug.Log("Performing Attack 3");
            // 실제 공격 로직 구현할 것.
            return Result.Success;
        }

        private Result PerformSpecialAttack()
        {
            Debug.Log("Performing Special Attack");
            // 실제 특수 공격 로직 구현할 것.
            return Result.Success;
        }
    }

}


