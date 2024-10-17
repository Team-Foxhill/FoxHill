using FoxHill.Core;
using FoxHill.Core.Damage;
using FoxHill.Core.Parry;
using FoxHill.Core.Pause;
using FoxHill.Core.Stat;
using FoxHill.Monster.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IState = FoxHill.Monster.FSM.IState;
using State = FoxHill.Monster.FSM.State;
using StateMachine = FoxHill.Monster.FSM.StateMachine;

namespace FoxHill.Monster.AI
{
    [RequireComponent(typeof(MonsterBehaviourTree))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(StateMachine))]
    public class SouthBossMonsterController : MonoBehaviour, IDamager, IDamageable, IStat, IStaggerable, IPausable
    {
        public Transform Transform => transform;

        public float MaxHp => 1000f;

        public float CurrentHp { get; private set; }

        public float MoveSpeed => _moveSpeed;

        public float Power => 10f;

        public float Defense => 3f;
        public bool IsFatalAttackable => _machine.CurrentState == State.Stagger;

        public event System.Action OnDead;
        [Header("AI Behaviours")]
        [Header("Seek")]
        [SerializeField] private float _seekRadius = 3.0f;
        [SerializeField] private float _seekAngle = 180f;
        [SerializeField] private int _seektargetLayer = 3;
        [SerializeField] private float _seekMaxDistance = 10.0f;
        [SerializeField] private float _maxDistanceFromOrigin = 20.0f;
        [SerializeField] private float _attackRange = 1.0f;
        [SerializeField] private float _specialAttackMinRange = 0.6f;
        [SerializeField] private float _specialAttackProbability = 0.3f;
        [SerializeField] private float _stoppingDistance = 0.3f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _directionUpdateInterval = 5f;
        [SerializeField] private float _horizontalAttackRange;
        [SerializeField] private float _verticalAttackRange;
        [SerializeField] private float _chargeAttackRange;
        [SerializeField] private float _jumpAttackRange;
        [SerializeField] private float _returnToOriginDelay;
        private LayerMask _playerMask;
        private LayerMask _towerMask;
        private bool _isNextAttackReady = true;
        private bool _isTargetInRange = false;
        private StateMachine _machine;
        private SpriteRenderer _spriteRenderer;
        private bool _isThisRunning;
        private Blackboard _blackboard;
        private bool _isPaused;


        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            IInputCommand inputCommand = new MonsterStateCommand();
            _machine = gameObject.AddComponent<StateMachine>();
            _machine.Initialize(new Dictionary<State, IState>()
            {
                { State.Idle, new IdleState(_machine, inputCommand) },
                { State.Move, new MoveState(_machine, inputCommand) },
                { State.AttackHorizontal, new AttackHorizontalState(_machine, inputCommand) },
                { State.AttackVertical, new AttackVerticalState(_machine, inputCommand) },
                { State.Jump, new JumpState(_machine, inputCommand) },
                { State.Dead, new DeadState(_machine, inputCommand) },
                { State.Stagger, new StaggerState(_machine, inputCommand) },
                { State.Charge, new ChargeState(_machine, inputCommand) }
                //{ State.Move, new MoveState(inputCommand) },
                //{ State.Jump, new JumpState(inputCommand) },
            });
            _playerMask = 1 << _seektargetLayer;
            _towerMask = 1 << 6;
            MakeBehaviourTree();
            CurrentHp = MaxHp;

            //gameObject.GetComponent<SpriteRenderer> ().enabled = false;
        }
        private void MakeBehaviourTree()
        {
            // 체이닝 함수를 활용하여 빌드.
            MonsterBehaviourTree tree = GetComponent<MonsterBehaviourTree>();
            tree.Build(this)
                .Selector(this)
                    .Sequence(this)
                        .Seek(tree, this, _maxDistanceFromOrigin, transform.right, _seekRadius, _seekAngle, _playerMask, _seekMaxDistance, _stoppingDistance, _moveSpeed, _directionUpdateInterval)
                        //.Decorator(this, () => CheckTargetRange(tree, _verticalAttackRange) && CheckNextAttackReady() && Random.value < _specialAttackProbability)/* 타겟이 범위 내에 있는지, 특수 공격 가능한 상태인지 확인*/
                        //.Execution(this, () => PerformAttack(tree, State.Charge))
                        //.Decorator(this, () => CheckTargetRange(tree, _horizontalAttackRange) && _machine.CurrentState != State.AttackHorizontal && Random.value < 0.3f)/* 타겟이 범위 내에 있는지 확인 */
                        //.Execution(this, () => PerformAction(tree, State.AttackHorizontal))
                        //.Decorator(this, () => CheckTargetRange(tree, _verticalAttackRange) && _machine.CurrentState != State.AttackVertical && Random.value < 0.3f)/* 타겟이 범위 내에 있는지 확인 */
                        //.Execution(this, () => PerformAction(tree, State.AttackVertical))
                        //.Decorator(this, () => CheckTargetRange(tree, _jumpAttackRange) && _machine.CurrentState != State.Jump && Random.value < 0.3f)/* 타겟이 범위 내에 있는지 확인 */
                        //.Execution(this, () => PerformAction(tree, State.Jump))
                        //.Decorator(this, () => CheckTargetRange(tree, _chargeAttackRange) && _machine.CurrentState != State.Charge && Random.value < 0.1f)/* 타겟이 범위 내에 있는지 확인 */
                        //.Execution(this, () => PerformAction(tree, State.Charge))
                        .Execution(this, () => PerformAction(tree, State.Stagger))
                    .FinishCurrentComposite()
                .FinishCurrentComposite();
            tree.blackboard.OriginPosition = transform.position;
            _blackboard = tree.blackboard;
            _blackboard.IsNextActionReady = true;
        }

        public void ChangeDirection(Vector2 direction = default)
        {
            //if (_machine.CurrentState != State.Idle)
            //{
            //    return;
            //}
            if (direction.x > Vector2.zero.x)
            {
                _spriteRenderer.flipX = false;
            }
            else if (direction.x < Vector2.zero.x)
            {
                _spriteRenderer.flipX = true;
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 타겟과의 범위를 확인하여 _isTargetInRange로 저장하고 블랙보드에 공유하는 메서드.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        private bool CheckTargetRange(MonsterBehaviourTree tree, float range)
        {
            if (tree.blackboard.Target == null)
            {
                _isTargetInRange = false;
                return _isTargetInRange;
            }
            Vector2 targetVector;
            if (tree.blackboard.Target)
            {
                targetVector = tree.blackboard.Target.position;
            }
            else
            {
                targetVector = tree.blackboard.Transform.position;
            }
            float distance = Vector2.Distance(tree.blackboard.Transform.position, targetVector);
            if (distance > range)
            {
                _isTargetInRange = false;
                return _isTargetInRange;
            }
            _isTargetInRange = true;
            return _isTargetInRange;
        }

        public bool IsAttackingNow()
        {
            if (_machine.CurrentState == State.AttackVertical || _machine.CurrentState == State.AttackHorizontal || _machine.CurrentState == State.Jump || _machine.CurrentState == State.Charge)
            {
                return true;
            }
            return false;
        }
        public bool IsStaggerNow()
        {
            if (_machine.CurrentState == State.Stagger)
            {
                return true;
            }
            return false;
        }

        public Result PerformAction(MonsterBehaviourTree tree, State type) // 점프 로직만 하나 만들어주면 끝날듯. + 공격할 때 살짝 플레이어 방향으로 이동하게 만들기.
        {
            if (!tree.blackboard.IsNextActionReady && _isThisRunning == false)
            {
                return Result.Failure;
            }

            if (_isThisRunning == false) // 공격중이 아니라면
            {
                tree.blackboard.IsNextActionReady = false;
                _isThisRunning = true;
                Vector2 targetVector;
                if (tree.blackboard.Target)
                {
                    targetVector = tree.blackboard.Target.position;
                }
                else
                {
                    targetVector = tree.blackboard.Transform.position;
                }
                ChangeDirection(targetVector - (Vector2)tree.blackboard.Transform.position);
                _machine.ChangeState(type);
                return Result.Running;
            }

            if (IsAttackingNow() == true) // 공격에 해당하는 애니메이션이라면.
            {
                Vector2 direction;
                Vector2 newPosition;
                if (tree.blackboard.Target)
                {
                    direction = ((Vector2)tree.blackboard.Target.position - (Vector2)tree.blackboard.Transform.position).normalized;
                    newPosition = (Vector2)transform.position + direction * (_moveSpeed / 4) * Time.deltaTime;
                }
                else
                {
                    direction = Vector2.zero;
                    newPosition = (Vector2)transform.position;
                }
                ChangeDirection(direction);
                if (IsValidPosition(newPosition))
                {
                    transform.position = newPosition;
                }
                // 공격 진행 중
                return Result.Running;
            }
            if (IsStaggerNow() == true) // Stagger중이라면.
            {
                //Stagger 진행중.
                return Result.Running;
            }
            else
            {
                // 행동 완료
                _isThisRunning = false;
                tree.blackboard.IsNextActionReady = true;
                return Result.Success;
            }
        }

        private Result PerformSpecialAttack()
        {
            // 실제 특수 공격 로직과 애니메이션 변경 구현할 것.
            _machine.ChangeState(State.Charge);
            return Result.Success;
        }


        public void TakeDamage(IDamager damager, float damage)
        {
            CurrentHp -= (damage - Defense);
            if (_blackboard.Target == null) // 현재 공격 대상이 없는 경우에만.
            {
                _blackboard.Target = damager.Transform; // 공격 대상으로 공격자를 설정.
                IDamageable damageable = damager.Transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.OnDead += ResetTarget;
                }
            }
            if (CurrentHp <= 0f)
            {
                Dead();
            }
        }

        private void ResetTarget()
        {
            _blackboard.Target = null;
        }

        public void Dead()
        {
            _machine.ChangeState(State.Dead);
        }

        AttackSpec tmp;
        private void PerformAttack(AnimationEvent animationEvent) // Todo.Villin 기즈모 만들기.
        {
            if (_isPaused)
            {
                return;
            }
            // 범위 판정.
            var attakcSpec = (AttackSpec)animationEvent.objectReferenceParameter;
            tmp = attakcSpec; // 기즈모용.
            RaycastHit2D[] hit2D = Physics2D.CircleCastAll((Vector2)transform.position + attakcSpec.CastCenterPosition, attakcSpec.Radius, attakcSpec.Direction, attakcSpec.Distance, _playerMask | _towerMask);
            foreach (var hit in hit2D) // 각도 확인하는 작업 필요.
            {
                // 여기서 콜라이더2D의 레이어 확인하기. => 대미저블이고 플레이어 혹은 타워라면 IsInSight진행, 아닐 경우는 패스.
                if (hit.transform.gameObject.TryGetComponent<IDamageable>(out var target) == false)
                {
                    continue;
                }
                if (1 << hit.transform.gameObject.layer == _playerMask) // 플레이어라면,
                {
                    IParryable parry = hit.transform.gameObject.GetComponent<IParryable>();

                    if (parry != null && parry.IsPerfectGuard == true)// 상대가 퍼펙트 가드 타이밍인지 확인.
                    {
                        _machine.ChangeState(State.Stagger);//stagger 상태 진입.
                    }
                    else
                    {
                        target.TakeDamage(this, attakcSpec.DamageMultiplier * Power);
                    }
                }
                else
                {
                    target.TakeDamage(this, attakcSpec.DamageMultiplier * Power);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if(tmp != null)
                Gizmos.DrawWireSphere((Vector2)transform.position + tmp.CastCenterPosition, tmp.Radius);
        }
        public void SetIdle()
        {
            _machine.ChangeState(State.Idle);
        }

        public void Move(Vector2 direction)
        {
            if (_isPaused)
            {
                return;
            }
            // 애니메이션 실행하기.
            if (_isNextAttackReady == false)
            {
                return;
            }

            Vector2 newPosition = (Vector2)transform.position + direction * _moveSpeed * Time.deltaTime;
            ChangeDirection(direction);
            _machine.ChangeState(State.Move);

            if (IsValidPosition(newPosition) && _machine.CurrentState == State.Move)
            {
                transform.position = newPosition;
            }
        }

        public bool IsValidPosition(Vector2 position)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
            foreach (var collider in colliders)
            {
                if (1 << collider.gameObject.layer == LayerMask.NameToLayer("BossMonster"))
                {
                    continue;
                }
                else if (1 << collider.gameObject.layer == LayerMask.NameToLayer("Water")) // todo.Villin 임시로 water 사용. 나중엔 obstacle과 같은 이름으로 변경할 것.
                {
                    return false;
                }
            }
            return true;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        public void MakeStun(IDamager damager)
        {
            throw new System.NotImplementedException();
        }
    }
}
