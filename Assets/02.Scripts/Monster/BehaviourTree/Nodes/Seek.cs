using FoxHill.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace FoxHill.Monster.AI
{
    public class Seek : Node
    {
        public Seek(MonsterBehaviourTree tree, SouthBossMonsterController controller, float maxDistanceFromOrigin, Vector2 checkDirection, float radius, float angle, LayerMask targetMask, float chaseDistance, float stoppingDistance, float moveSpeed, float directionUpdateInterval) : base(tree, controller)
        {
            _maxDistanceFromOrigin = maxDistanceFromOrigin;
            _radius = radius;
            _angle = angle;
            _targetMask = targetMask;
            _chaseDistance = chaseDistance;
            _stoppingDistance = stoppingDistance;
            _moveSpeed = moveSpeed;
            _checkDirection = checkDirection;
            _directionUpdateInterval = directionUpdateInterval;
            _controller = controller;
        }


        private SouthBossMonsterController _controller;
        private float _maxDistanceFromOrigin; // 타겟을 추적중일 때 몬스터가 타겟의 위치를 알 수 있는 최대 거리.(radius보다 길어야 한다.)
        private float _radius; // 타겟을 탐지할 때 인식할 수 있는 거리. _resetDistance보다 짧아야 한다.(인식 범위 가지고 장난치는 걸 막기 위해.)
        private float _angle; // 시야각(degree). 캐릭터의 전방을 기준으로 이 각도 내에 있는 타겟만 감지합니다.
        private LayerMask _targetMask; // 타겟으로 인식할 수 있는 게임 오브젝트의 레이어.
        private float _chaseDistance; // 타겟을 추적중일 때 몬스터가 타겟의 위치를 알 수 있는 최대 거리.(radius보다 길어야 한다.)
        private float _stoppingDistance; // 타겟에 접근했을 때 멈출 거리. 이 거리 이내로 타겟에 접근하면 이동을 멈추고 'Success'를 반환.
        private float _moveSpeed; // 타겟을 향해 이동할 때의 속도.
        private Vector2 _checkDirection; // 시야를 확인할 기준 방향. 이 방향을 중심으로 _angle 만큼의 시야각 내에서 타겟을 탐지합니다.
        private float _directionUpdateTimer = 0f;
        private float _directionUpdateInterval = 5f;
        private float _returnToOriginDelay = 3f;
        private float _lostTargetTimer;
        private bool _isWaiting = false;
        private bool _isReturning = false;


        public override Result Invoke()
        {
            if (blackboard.Target)
            {
                return CaseTargetFounded();
            }
            // 타겟이 설정되지 않은 경우.
            else
            {
                return CaseTargetNotFounded();
                // 그 외의 경우라면.
            }
        }

        private Result CaseTargetFounded()
        {
            // 타겟이 이미 설정된 경우.
            _lostTargetTimer = 0f; // 타이머 리셋.
            float distance = Vector2.Distance(blackboard.Transform.position, blackboard.Target.position); // 거리는 타겟과 이 트랜스폼의 사이의 것을 사용.

            if (distance <= _stoppingDistance) // 만약 멈추는 거리보다 가깝다면,
            {
                return Result.Success; // 아무 것도 하지 않음.
            }
            else if (distance < _chaseDistance) // 쫓아갈 수 있는 거리 내에 있다면.
            {
                TryMove(); // 이동을 시도.
                return Result.Running;
            }
            else // 쫓아갈 수 있는 범위 밖이라면,
            {
                blackboard.Target = null; // 타겟을 초기화하고.
                return Result.Failure; // 실패를 반환.
            }
        }

        private Result CaseTargetNotFounded()
        {
            Collider2D[] colliders2D = Physics2D.OverlapCircleAll(blackboard.Transform.position, _radius, _targetMask);
            _directionUpdateTimer += Time.deltaTime;
            if (_directionUpdateTimer >= _directionUpdateInterval)
            {
                _checkDirection = GetRandomDirection();
                _controller.ChangeDirection(_checkDirection);
                _directionUpdateTimer = 0f; // 타이머 리셋
            }

            foreach (var collider in colliders2D)
            {

                // 여기서 콜라이더2D의 레이어 확인하기. => 플레이어 레이어라면 IsInSight진행, 아닐 경우는 패스.
                if (1 << collider.gameObject.layer != _targetMask)
                {
                    continue;
                }
                if (IsInSight(collider.transform, _checkDirection)) // 시야 내에 콜라이더가 있다면.
                {
                    blackboard.Target = collider.transform; // 타겟을 설정하고 이동 시도.
                    TryMove();
                    return Result.Running;
                }
            }

            float distance = Vector2.Distance(blackboard.Transform.position, blackboard.OriginPosition);
            if (distance > _stoppingDistance && _isReturning == false)
            {
                _lostTargetTimer += Time.deltaTime;
            }

            if (_isWaiting && _lostTargetTimer > _returnToOriginDelay)
            {
                _isWaiting = false;
            }

            if (_lostTargetTimer < _returnToOriginDelay && _isReturning == false) // 타겟을 잃은 직후에는 타겟을 찾은 후 없다면 잠시 정지.
            {
                _isWaiting = true;
                DebugFox.Log($"now lostTargetTimer value is {_lostTargetTimer}");
                _controller.SetIdle();
                return Result.Running;
            }

            if (distance <= _stoppingDistance) // 만약 원점과 가깝다면,
            {
                _isReturning = false;
                _controller.SetIdle();
                DebugFox.Log("Seek Success");
                return Result.Success; // 아무 것도 하지 않음.
            }
            else if (distance >= _maxDistanceFromOrigin || _isWaiting == false) // 만일 원점과의 거리가 최대 거리보다 멀거나, 대기 시간이 지났다면.
            {
                DebugFox.Log($"now lostTargetTimer reseted as {_lostTargetTimer} and Try Get Back to Origin");

                TryMove(); // 이동을 시도(원점으로).
                return Result.Running;
            }
            else
                return Result.Failure;
        }

        private void TryMove()
        {
            if ((Vector2)blackboard.Transform.position == blackboard.OriginPosition && blackboard.Target == null) // 원점에 있고, 타겟이 없다면.
            {
                return; // 아무 것도 하지 않음.
            }
            Vector2 direction;
            if (blackboard.Target == null && _isWaiting == false) // 타겟이 없고 복귀 시간이 되었다면
            {
                if (_isReturning == false)
                {
                    _isReturning = true;
                }
                _lostTargetTimer = 0;
                direction = ((Vector2)blackboard.OriginPosition - (Vector2)blackboard.Transform.position).normalized; // 원점을 향해 방향 설정.
            }
            else // 만일 타겟이 있다면
            {
                direction = ((Vector2)blackboard.Target.position - (Vector2)blackboard.Transform.position).normalized; // 타겟을 향해 방향 설정.
                blackboard.NowDirection = direction;
            }
            _controller.Move(direction);
        }

        private Vector2 GetRandomDirection()
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    return Vector2.right;
                case 1:
                    return Vector2.left;
                //case 2: DebugFox.Log("checkDirection is up"); 
                //    return Vector2.up;
                //case 3: DebugFox.Log("checkDirection is down"); 
                //    return Vector2.down;
                default: return Vector2.zero;
            }
        }

        /// <summary>
        /// 시야에 목표물이 있는지 확인하는 메서드.
        /// </summary>
        /// <param name="target">시야 내에 있는지 확인하고자 하는 트랜스폼.</param>
        /// <param name="checkDirection">확인할 방향.</param>
        /// <returns></returns>
        private bool IsInSight(Transform target, Vector2 checkDirection)
        {
            Vector2 origin = blackboard.Transform.position; // 트랜스폼이 부착된 개체에서 위치를 받아옴.
            Vector2 lookDir = ((Vector2)target.position - origin).normalized; // 타겟과 몬스터 간의 방향 벡터를 계산.
            float theta = Vector2.Angle(checkDirection, lookDir); // 방향 벡터(lookDir)를 checkDirection 방향과 계산했을 때의 각도로 변환.
            // 만일 시야각을 절반으로 나눈 것보다 두 개체 사이의 각도가 작다면.
            if (theta < _angle / 2.0f)
            {
                float distance = Vector2.Distance(origin, target.position); // 몬스터와 타겟의 거리.
                RaycastHit2D hit = Physics2D.Raycast(origin, lookDir, distance, _targetMask); // 타겟과 몬스터 사이의 거리와 방향에 맞춰서 레이캐스트로 타겟 마스크를 확인.

                //만일 hit의 콜라이더가 null이 아니라면.
                if (hit.collider != null)
                {
                    // Collider2D의 transform이 타겟과 동일한지 체크.
                    if (hit.transform == target && 1 << target.gameObject.layer == _targetMask)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //private Result CheckResetPosition()
        //{
        //    // 만일 타겟이 있고, 최대 거리를 벗어났거나, 타겟이 없고, 원점으로 인정되지 않는 거리에 있는 경우.
        //    if ((blackboard.target != null && Vector2.Distance((Vector2)blackboard.transform.position, blackboard.originPosition) > _maxDistanceFromOrigin) ||
        //        (blackboard.target == null && Vector2.Distance((Vector2)blackboard.transform.position, blackboard.originPosition) > _stoppingDistance))
        //    {
        //        // 타겟을 초기화하고, 원점으로 몬스터를 이동시킴.
        //        blackboard.target = null;
        //        TryMove();
        //        return Result.Running;
        //    }
        //    // 그 외의 경우 실패를 반환.
        //    return Result.Failure;
        //}

    }
}