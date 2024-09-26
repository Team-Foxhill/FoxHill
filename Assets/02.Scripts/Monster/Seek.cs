using Unity.Mathematics;
using UnityEngine;

namespace FoxHill.Monster.AI
{
    public class Seek : Node
    {
        /// <summary>
        /// AI가 목표를 탐지하고 추적하도록 신호를 보냅니다.
        /// </summary>
        /// <param name="tree">행동 트리 인스턴스</param>
        /// <param name="radius">탐지 반경</param>
        /// <param name="angle">시야각</param>
        /// <param name="targetMask">목표 레이어 마스크</param>
        /// <param name="maxDistance">최대 추적 거리</param>
        public Seek(MonsterBehaviourTree tree, float radius, float angle, int targetMask, float maxDistance) : base(tree)
        {
            _radius = radius;
            _angle = angle;
            _targetLayer = targetMask;
            _maxDistance = maxDistance;
        }

        private float _radius; // 목표 탐지 반경.
        private float _angle; // AI의 시야각.
        private int _targetLayer; // 목표 레이어.
        private float _maxDistance; // 최대 추적 거리.

        /// <summary>
        /// Seek을 실행한 결과를 반환하는 메서드.
        /// </summary>
        /// <returns><see cref="Result"></see></returns>
        public override Result Invoke()
        {
            // 감지된 목표가 있다면.
            if (blackboard.target)
            {
                float distance = Vector2.Distance(blackboard.transform.position,
                                                  blackboard.target.position);
                // 목표에 도달했다면.
                if (distance <= blackboard.agent.EntityLocomotion.StoppingDistance)
                {
                    return Result.Success;
                }
                // 목표를 추적중이라면.
                else if (distance < _maxDistance)
                {
                    SetDestination(blackboard.target.position);
                    return Result.Running;
                }
                // 목표가 범위 밖에 있다면.
                else
                {
                    blackboard.target = null;
                    blackboard.agent.SetDestination(blackboard.transform.position); // todo.Villin 현재는 임시로 그 자리에서 멈춰서도록 변경. 원래 자리로 돌아가는 로직은 추후 구현.
                    return Result.Failure;
                }
            }
            // 감지된 목표가 없다면.
            else
            {
                Collider2D[] cols = Physics2D.OverlapCircleAll(blackboard.transform.position, _radius, _targetLayer); // 주변을 원형으로 범위만큼 덮어서 타겟 마스크에 해당하는 게임오브젝트를 찾아 어레이로 만든다.(타겟 마스크만 잘 설정해주면 몬스터끼리도 타겟으로 지정할 수 있다.)
                if (cols.Length <= 0) // 하나도 못 찾았거나 오류가 났다면.
                {
                    return Result.Failure;
                }

                for (int i = 0; i < cols.Length; i++)
                {
                    if (IsInSight(cols[i].transform)) // 시야 내에 있다면.
                    {
                        blackboard.target = cols[0].transform;
                        SetDestination(blackboard.target.position); // 그 대상을 목표로 지정.
                        return Result.Running;
                    }
                }

                return Result.Failure; // 시야 내에 아무도 없다면 실패 반환.
            }
        }

        private bool IsInSight(Transform target)
        {
            Vector2 origin = blackboard.transform.position;
            Vector2 direction = blackboard.transform.right; // AI의 전방(오른쪽) 벡터.
            Vector2 lookDir = ((Vector2)target.position - origin).normalized;
            float theta = Mathf.Acos(Vector2.Dot(direction, lookDir)) * Mathf.Rad2Deg;

            if (theta < _angle / 2.0f) // 계산된 각도가 시야각 절반보다 작다면.
            {
                RaycastHit2D hit = Physics2D.Raycast(origin, lookDir, Vector2.Distance(origin, target.position), _targetLayer);
                return hit.collider != null && hit.transform == target; // 장애물이 가로막지 않고 있으며, 콜라이더가 존재한다면 true를 반환.
            }

            return false;
        }

        /// <summary>
        /// AI의 목적지를 설정하는 메서드.
        /// </summary>
        /// <param name="destination">목적지 위치.</param>
        private void SetDestination(Vector2 destination)
        {
            if (blackboard.agent != null)
            {
                float3 destination3D = new Vector3(destination.x, destination.y, 0f); // Vector2를 float3로 변환하여 전달.
                blackboard.agent.SetDestination(destination3D);
            }
        }
    }
}
