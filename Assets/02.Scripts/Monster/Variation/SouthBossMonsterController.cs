using FoxHill.Core;
using FoxHill.Core.Damage;
using FoxHill.Core.Parry;
using FoxHill.Core.Pause;
using FoxHill.Core.Stat;
using FoxHill.Items;
using FoxHill.Monster.FSM;
using FoxHill.Player;
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

        public float MaxHp { get; private set; }

        public float CurrentHp { get; private set; }

        public float MoveSpeed => _moveSpeed;

        public float Power => _power;

        public float Defense => _defense;
        public bool IsFatalAttackable => _machine.CurrentState == State.Stagger;
        public event System.Action OnDead;
        [Header("BossMonsterStat")]
        [SerializeField] private float _maxHP = 500;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _power = 10f;
        [SerializeField] private float _defense = 3f;
        [Header("AI Behaviours")]
        [Header("Seek")]
        [SerializeField] private float _seekRadius = 3.0f;
        [SerializeField] private float _seekAngle = 180f;
        [SerializeField] private int _seektargetLayer = 3;
        [SerializeField] private float _seekMaxDistance = 10.0f;
        [SerializeField] private float _maxDistanceFromOrigin = 20.0f;
        [SerializeField] private float _specialAttackMinRange = 0.6f;
        [SerializeField] private float _attackRange = 1.0f;
        [SerializeField] private float _stoppingDistance = 0.3f;
        [SerializeField] private float _reChaseStartDistance = 7f;
        [SerializeField] private float _directionUpdateInterval = 5f;
        [SerializeField] private float _horizontalAttackRange;
        [SerializeField] private float _verticalAttackRange;
        [SerializeField] private float _chargeAttackRange;
        [SerializeField] private float _jumpAttackRange;
        [SerializeField] private float _returnToOriginDelay;
        [Header("AttackProbability")]
        [SerializeField] private float _specialAttackProbability = 0.3f;
        [SerializeField] private float _horizontalAttackProbability = 0.3f;
        [SerializeField] private float _verticalAttackProbability = 0.3f;
        [SerializeField] private float _jumpAttackProbability = 0.3f;
        [SerializeField] private SouthBossMonsterSubController _subController;

        private readonly Color COLOR_DAMAGED = new Color(255f / 255f, 47f / 255f, 47f / 255f);
        private readonly WaitForSeconds _colorChangeWait = new WaitForSeconds(0.2f);

        private CapsuleCollider2D _collider;
        private LayerMask _playerMask;
        private LayerMask _towerMask;
        private bool _isNextAttackReady = true;
        private bool _isTargetInRange = false;
        private StateMachine _machine;
        private SpriteRenderer _spriteRenderer;
        private bool _isActionRunning;
        private Blackboard _blackboard;
        private bool _isPaused;
        private bool _isDead = false;
        private IDamager _stunInvoker;
        private Color _initialColor;


        private void Awake()
        {
            _subController.OnJumpAttack += PerformAttack;
            PauseManager.Register(this);
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
                { State.KnockBack, new KnockBackState(_machine, inputCommand) },
                { State.AfterShock, new AfterShockState(_machine, inputCommand) },
                { State.Charge, new ChargeState(_machine, inputCommand) }
                //{ State.Move, new MoveState(inputCommand) },
                //{ State.Jump, new JumpState(inputCommand) },
            });
            _playerMask = 1 << _seektargetLayer;
            _towerMask = 1 << 6;
            MakeBehaviourTree();
            MaxHp = _maxHP;
            CurrentHp = MaxHp;
            _initialColor = _spriteRenderer.color;
            _collider = gameObject.GetComponent<CapsuleCollider2D>();
        }

        private void MakeBehaviourTree()
        {
            // 체이닝 함수를 활용하여 빌드.
            MonsterBehaviourTree tree = GetComponent<MonsterBehaviourTree>();
            tree.Build(this)
                .Selector()
                        .Execution(() => CheckDead())
                        .Decorator(() => tree.Blackboard.IsStunInvoked || tree.Blackboard.IsStunRunning)
                        .Execution(() => PerformStun(tree))
                    .Sequence()
                        .Seek(tree, _maxDistanceFromOrigin, transform.right, _seekRadius, _seekAngle, _playerMask, _seekMaxDistance, _stoppingDistance, _reChaseStartDistance, _moveSpeed, _directionUpdateInterval)
                            .Parallel(1)
                                .Decorator(() => CheckTargetRange(tree, _horizontalAttackRange) && _machine.CurrentState != State.AttackHorizontal && Random.value < _horizontalAttackProbability && tree.Blackboard.IsStunInvoked == false && tree.Blackboard.IsStunRunning == false)
                                .Execution(() => PerformAction(tree, State.AttackHorizontal))
                                .Decorator(() => CheckTargetRange(tree, _verticalAttackRange) && _machine.CurrentState != State.AttackVertical && Random.value < _verticalAttackProbability && tree.Blackboard.IsStunInvoked == false && tree.Blackboard.IsStunRunning == false)
                                .Execution(() => PerformAction(tree, State.AttackVertical))
                                .Decorator(() => CheckTargetRange(tree, _jumpAttackRange) && _machine.CurrentState != State.Jump && Random.value < _jumpAttackProbability && tree.Blackboard.IsStunInvoked == false && tree.Blackboard.IsStunRunning == false)
                                .Execution(() => PerformAction(tree, State.Jump))
                            .FinishCurrentComposite()
                            .Parallel(1)
                                .Decorator(() => CheckTargetRange(tree, _horizontalAttackRange) && _machine.CurrentState != State.AttackHorizontal && Random.value < _horizontalAttackProbability && tree.Blackboard.IsStunInvoked == false && tree.Blackboard.IsStunRunning == false)
                                .Execution(() => PerformAction(tree, State.AttackHorizontal))
                                .Decorator(() => CheckTargetRange(tree, _verticalAttackRange) && _machine.CurrentState != State.AttackVertical && Random.value < _verticalAttackProbability && tree.Blackboard.IsStunInvoked == false && tree.Blackboard.IsStunRunning == false)
                                .Execution(() => PerformAction(tree, State.AttackVertical))
                                .Decorator(() => CheckTargetRange(tree, _jumpAttackRange) && _machine.CurrentState != State.Jump && Random.value < _jumpAttackProbability && tree.Blackboard.IsStunInvoked == false && tree.Blackboard.IsStunRunning == false)
                                .Execution(() => PerformAction(tree, State.Jump))
                            .FinishCurrentComposite()
                            .Parallel(1)
                                .Decorator(() => CheckTargetRange(tree, _chargeAttackRange) && _machine.CurrentState != State.Charge && Random.value < _specialAttackProbability)
                                .Execution(() => PerformAction(tree, State.Charge))
                                .Decorator(() => CheckTargetRange(tree, _horizontalAttackRange) && _machine.CurrentState != State.AttackHorizontal && Random.value < _horizontalAttackProbability && tree.Blackboard.IsStunInvoked == false && tree.Blackboard.IsStunRunning == false)
                                .Execution(() => PerformAction(tree, State.AttackHorizontal))
                                .Decorator(() => CheckTargetRange(tree, _verticalAttackRange) && _machine.CurrentState != State.AttackVertical && Random.value < _verticalAttackProbability && tree.Blackboard.IsStunInvoked == false && tree.Blackboard.IsStunRunning == false)
                                .Execution(() => PerformAction(tree, State.AttackVertical))
                                .Decorator(() => CheckTargetRange(tree, _jumpAttackRange) && _machine.CurrentState != State.Jump && Random.value < _jumpAttackProbability && tree.Blackboard.IsStunInvoked == false && tree.Blackboard.IsStunRunning == false)
                                .Execution(() => PerformAction(tree, State.Jump))
                            .FinishCurrentComposite()
                        .FinishCurrentComposite()
                    .FinishCurrentComposite();
            tree.Blackboard.OriginPosition = transform.position;
            _blackboard = tree.Blackboard;
            _blackboard.IsNextActionReady = true;
        }

        public void Move(Vector2 direction)
        {
            // 애니메이션 실행하기.
            if (_isNextAttackReady == false || _blackboard.IsStunRunning)
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

        public void ChangeDirection(Vector2 direction = default)
        {

            if (_machine.CurrentState == State.Dead && _isDead)
            {
                return;
            }
            if (_machine.CurrentState == State.Dead && _isDead == false)
            {
                _isDead = true;
            }

            if (direction.x > Vector2.zero.x)
            {
                _spriteRenderer.flipX = false;
            }
            else if (direction.x < Vector2.zero.x)
            {
                _spriteRenderer.flipX = true;
            }
            else // 0인 경우 .
            {
                return;
            }

        }

        private bool CheckTargetRange(MonsterBehaviourTree tree, float range)
        {
            if (tree.Blackboard.Target == null)
            {
                _isTargetInRange = false;
                return _isTargetInRange;
            }
            Vector2 targetVector;
            if (tree.Blackboard.Target)
            {
                targetVector = tree.Blackboard.Target.position;
            }
            else
            {
                targetVector = tree.Blackboard.Transform.position;
            }
            float distance = Vector2.Distance(tree.Blackboard.Transform.position, targetVector);
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

        public Result PerformAction(MonsterBehaviourTree tree, State type) // 점프 로직만 하나 만들어주면 끝날듯.
        {
            if (!tree.Blackboard.IsNextActionReady && _isActionRunning == false)
                return Result.Failure;

            if (_isActionRunning == false && _machine.CurrentState != State.Dead) // 공격중이거나 죽은 상태가 아니라면
            {
                tree.Blackboard.IsNextActionReady = false;
                _isActionRunning = true;
                Vector2 targetVector;
                if (tree.Blackboard.Target)
                {
                    targetVector = tree.Blackboard.Target.position;
                }
                else
                {
                    targetVector = tree.Blackboard.Transform.position;
                }
                ChangeDirection(targetVector - (Vector2)tree.Blackboard.Transform.position);
                _machine.ChangeState(type);
                return Result.Running;
            }

            if (IsAttackingNow() == true) // 공격에 해당하는 애니메이션이라면.
            {
                if (tree.Blackboard.IsStunRunning)
                {
                    return Result.Failure;
                }
                Vector2 direction;
                Vector2 newPosition;
                if (tree.Blackboard.Target)
                {
                    direction = ((Vector2)tree.Blackboard.Target.position - (Vector2)tree.Blackboard.Transform.position).normalized;
                    newPosition = (Vector2)transform.position + direction * (_moveSpeed / 4) * Time.deltaTime;
                }
                else
                {
                    direction = Vector2.zero;
                    newPosition = (Vector2)transform.position;
                }
                ChangeDirection(direction);
                if (IsValidPosition(newPosition) && _isPaused == false)
                {
                    transform.position = newPosition;
                }
                // 공격 진행 중
                return Result.Running;
            }
            if (_machine.CurrentState == State.Stagger) // Stagger중 혹은 AfterShock 중이라면.
            {
                if (tree.Blackboard.IsStunInvoked)
                {
                    return Result.Failure;
                }
                //Stagger 진행중.
                return Result.Running;
            }
            else
            {
                // 행동 완료
                _isActionRunning = false;
                tree.Blackboard.IsNextActionReady = true;
                return Result.Success;
            }
        }

        public void OnJumpActionStart() // 애니메이션 이벤트로 실행되는 메서드.
        {
            DebugFox.Log("애니메이션 실행 시도!");
            _subController.PlayAnimation();
        }

        // todo.Villin 이어서 작업하기.(점프 공격)
        public void OnJumpUp() // 애니메이션 이벤트로 실행되는 메서드.
        {
            StartCoroutine(MoveDirectionExecutor(Vector2.up, 0.5f, false));//이동및 스프라이트 렌더러 비활성 처리.
        }

        public void OnJumpDown() // 애니메이션 이벤트로 실행되는 메서드.
        {

            StartCoroutine(LerpPositionExecutor(_blackboard.Target.position, 0.1f, true));//이동및 스프라이트 렌더러 비활성 처리.
        }

        private IEnumerator MoveDirectionExecutor(Vector2 direction, float time, bool isEnable)
        {
            _collider.enabled = isEnable;
            float elapsedTime = 0;
            while (time > elapsedTime)
            {
                elapsedTime += Time.deltaTime;
                //transform.position = (Vector2)transform.position + direction * _moveSpeed * elapsedTime; // todo.Villin 화면 위로 사라졌다가 돌아오는 스프라이트 렌더러 오브젝트 하나 따로 만들고, 대신 실제 몬스터는 그림자 애니메이션만 실행하기.
                if (Vector2.Distance(transform.position, _blackboard.OriginPosition) > (_maxDistanceFromOrigin - 1f))
                {
                    transform.position = _blackboard.OriginPosition;
                }
                yield return null;
            }
        }

        private IEnumerator LerpPositionExecutor(Vector2 position, float duration, bool isEnable)
        {
            position.y += 1f;
            float elapsedTime = 0;
            while (duration > elapsedTime)
            {
                elapsedTime += Time.deltaTime;

                float t = Mathf.Clamp01(elapsedTime / duration);
                if (Vector2.Distance(transform.position, position) < 0.1f)
                {
                    _collider.enabled = isEnable;
                    yield break;
                }
                transform.position = Vector2.Lerp(transform.position, position, t);
                yield return null;
            }
        }

        public void TakeDamage(IDamager damager, float damage)
        {
            StartCoroutine(C_ChangeColor());
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
                ChangeDirection();
                return;
            }
        }

        private IEnumerator C_ChangeColor()
        {
            _spriteRenderer.color = COLOR_DAMAGED;
            yield return _colorChangeWait;
            _spriteRenderer.color = _initialColor;
        }

        private void ResetTarget()
        {
            _blackboard.Target = null;
        }

        private Result CheckDead()
        {
            if (CurrentHp > 0)
                return Result.Failure;
            else
            {
                Dead();
                return Result.Success;
            }
        }

        public void Dead()
        {
            _blackboard.IsDead = true;
            _machine.ChangeState(State.Dead);
            DropManager.Instance.DropExp(this.transform, 1000f);
            DropManager.Instance.DropItem(this.transform);
            transform.GetComponent<CapsuleCollider2D>().enabled = false;
            // todo.Villin 아이템 드랍 추가하기.
        }

        private AttackSpec tmp;
        private Vector2 adjustedCenterPosition;
        private void PerformAttack(AnimationEvent animationEvent) // 애니메이션 이벤트로 받는 메서드.
        {
            // 범위 판정.
            var attackSpec = (AttackSpec)animationEvent.objectReferenceParameter;
            AttackSpec attackSpecChanged = attackSpec;
            tmp = attackSpecChanged; // 기즈모용.
            if (_spriteRenderer.flipX)
            {
                adjustedCenterPosition = attackSpec.CastCenterPosition - new Vector2(2f, 0);
                if (_machine.CurrentState == State.AttackVertical)
                {
                    adjustedCenterPosition -= new Vector2(.86f, 0);
                }
            }
            else
            {
                adjustedCenterPosition = attackSpec.CastCenterPosition;
            }
            RaycastHit2D[] hit2D = Physics2D.CircleCastAll((Vector2)transform.position + adjustedCenterPosition, attackSpec.Radius, attackSpec.Direction, attackSpec.Distance, _playerMask | _towerMask);
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
                        DebugFox.Log($"perfectguard state is {parry.IsPerfectGuard}");
                        _machine.ChangeState(State.Stagger);//stagger 상태 진입.
                    }
                    else
                    {
                        target.TakeDamage(this, attackSpec.DamageMultiplier * Power);
                    }
                }
                else
                {
                    target.TakeDamage(this, attackSpec.DamageMultiplier * Power);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (tmp != null)
                Gizmos.DrawWireSphere((Vector2)transform.position + adjustedCenterPosition, tmp.Radius);
        }

        public void SetIdle()
        {
            _machine.ChangeState(State.Idle);
        }

        public void MakeStun(IDamager damager)
        {
            _stunInvoker = damager;
            _blackboard.IsStunInvoked = true;
            DebugFox.Log($"_blackboard.IsStunRunning is now {_blackboard.IsStunRunning}, _isStun is now {_blackboard.IsStunInvoked}");
        }

        private Result PerformStun(MonsterBehaviourTree tree)
        {
            if (tree.Blackboard.IsStunRunning == false && tree.Blackboard.IsStunInvoked == true)
            {
                DebugFox.Log("stun Coroutine Started!");
                tree.Blackboard.IsStunInvoked = false;
                StartCoroutine(KnockBack());
                tree.Blackboard.IsNextActionReady = false;
                return Result.Running;
            }
            else if (tree.Blackboard.IsStunRunning == true && tree.Blackboard.IsStunInvoked == true)
            {
                DebugFox.Log("stun has already invoked!");
                tree.Blackboard.IsStunInvoked = false;
                return Result.Running;
            }
            else if (tree.Blackboard.IsStunRunning == true && tree.Blackboard.IsStunInvoked == false)
            {
                DebugFox.Log("stun Coroutine Running!");
                tree.Blackboard.IsNextActionReady = false;
                return Result.Running;
            }
            else if (tree.Blackboard.IsStunRunning == false && tree.Blackboard.IsStunInvoked == false)
            {
                DebugFox.Log("stun Success!");
                tree.Blackboard.IsNextActionReady = true;
                return Result.Success;
            }
            else
                DebugFox.Log("stun Failure!");
            return Result.Failure;
        }

        private IEnumerator KnockBack()
        {
            _blackboard.IsStunRunning = true;
            DebugFox.Log($"ChangeState as Stun, now State is {_machine.CurrentState.ToString()}!");
            while (_machine.CurrentState != State.KnockBack)
            {
                _machine.ChangeState(State.KnockBack);
                yield return null;
            }
            while (_machine.CurrentState == State.KnockBack)
            {
                DebugFox.Log($"State is {_machine.CurrentState.ToString()}, so While is running!");
                if (_isPaused == true)
                {
                    yield return new WaitUntil(() => _isPaused == false);
                }
                Vector2 direction = ((Vector2)transform.position - (Vector2)_stunInvoker.Transform.position).normalized;
                transform.position = (Vector2)transform.position + direction * _moveSpeed * Time.deltaTime;
                DebugFox.Log($"direction is {direction}, transform.position is {transform.position.x}, {transform.position.y}");
                yield return null;
            }
            _blackboard.IsStunRunning = false;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }
    }
}
