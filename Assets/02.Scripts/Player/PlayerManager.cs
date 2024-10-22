using FoxHill.Controller;
using FoxHill.Core.Damage;
using FoxHill.Core.Knockback;
using FoxHill.Core.Parry;
using FoxHill.Core.Pause;
using FoxHill.Core.Utils;
using FoxHill.Player.Data;
using FoxHill.Player.Inventory;
using FoxHill.Player.Quest;
using FoxHill.Player.Stat;
using FoxHill.Player.State;
using FoxHill.Quest;
using FoxHill.Tower;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FoxHill.Player
{
    /// <summary>
    /// 플레이어와 관련된 이벤트 및 데이터, 상태 등을 전반적으로 관리합니다.
    /// </summary>
    public class PlayerManager : CharacterControllerBase, IDamageable, IDamager, IKnockbackable, IParryable
    /// 플레이어와 직접적으로 관련된 클래스(ex. HPController, InputController)는 PlayerManager를 가지고 있고
    /// 플레이어와 연관된 외부 시스템 클래스(ex. Quest, Inventory)는 PlayerManager가 가지고 있는 형태입니다.
    {
        [HideInInspector] public UnityEvent<float> OnPlayerDamaged;
        public event Action OnDead;
        [HideInInspector] public UnityEvent OnSwitchSkill;
        [HideInInspector] public UnityEvent OnCastSkill;

        [HideInInspector] public UnityEvent OnOpenInventory;
        [HideInInspector] public UnityEvent OnCloseInventory;
        [HideInInspector] public UnityEvent<Vector2> OnSwitchSlotInventory;
        [HideInInspector] public UnityEvent OnSelectInventory;
        [HideInInspector] public UnityEvent OnDeselectInventory;

        [HideInInspector] public UnityEvent OnEnterSpawn;
        [HideInInspector] public UnityEvent OnExitSpawn;
        [HideInInspector] public UnityEvent<Vector2> OnMovePrefabSpawn;
        [HideInInspector] public UnityEvent OnConfirmSpawn;
        [HideInInspector] public UnityEvent OnCancelSpawn;

        [HideInInspector] public UnityEvent OnReset; // GameScene 진입 시 Invoke


        [HideInInspector] public UnityEvent<List<Reward>> OnClearQuest;
        [HideInInspector] public UnityEvent OnKillMonster;
        [HideInInspector] public UnityEvent<int> OnEncounterNPC;


        public bool IsInventoryOpen => _isInventoryOpen;
        public bool IsPaused => _isPaused;
        public bool IsDead => _isDead;
        public bool IsTowerSpawnerOpen => _isTowerSpawnerOpen;
        public bool IsMovable // 플레이어가 움직일 수 있는지 확인
        {
            get => (State.CurrentActionState != PlayerState.Knockback)
                && (State.CurrentActionState != PlayerState.Dodge);
        }
        public bool IsActable { get => State.CurrentActionState == PlayerState.None; } // 플레이어가 Action을 수행할 수 있는지 확인
        public bool IsOnKnockback { get => State.CurrentActionState == PlayerState.Knockback; } // 플레이어가 넉백을 당하고 있는지 확인
        public bool IsPerfectGuard { get; set; } = false;

        /// <summary>
        /// 플레이어가 X축으로 바라보는 방향
        /// </summary>
        public bool IsLeftward => _isLeftward;

        public Vector2 Direction
        {
            get => _direction;
            set
            {
                if (value == Vector2.zero)
                    return;

                _direction = value;

                if (_direction.x > 0)
                {
                    _isLeftward = false;
                }
                else if (_direction.x < 0)
                {
                    _isLeftward = true;
                }
            }
        }

        public Vector2 MoveInput { get; set; }


        public PlayerStat Stat { get; private set; }
        public PlayerStateManager State { get; private set; }
        public CharacterController CharacterController { get; private set; }
        public PlayerInventory Inventory { get; private set; }
        public TowerManager Tower { get; private set; }

        public Transform Transform => transform;


        [SerializeField] private PlayerData _data;
        [SerializeField] private LevelTable _levelTable;

        private Vector2 _direction = Vector2.right;
        private bool _isLeftward = false;

        private bool _isInventoryOpen = false;
        private bool _isTowerSpawnerOpen = false;
        private bool _isPlayerDodging = false;

        protected override void Awake()
        {
            base.Awake();

            CharacterController = GetComponent<CharacterController>();

            Stat = new PlayerStat(_data, _levelTable);
            State ??= FindFirstObjectByType<PlayerStateManager>();
            Inventory ??= FindFirstObjectByType<PlayerInventory>();
            Tower ??= FindFirstObjectByType<TowerManager>();
        }

        private void Start()
        {
            // 외부 시스템들과 연관된 이벤트들 관리 (ex. Inventory, Tower)
            // Inventory
            OnOpenInventory?.AddListener(() =>
            {
                _isInventoryOpen = true;
                Inventory.ToggleCanvas(true);
                PauseManager.Pause();
            });

            OnCloseInventory?.AddListener(() =>
            {
                _isInventoryOpen = false;
                Inventory.ToggleCanvas(false);
                PauseManager.Resume();
            });

            OnSwitchSlotInventory?.AddListener((input) =>
            {
                Inventory.SwitchSlot(input);
            });

            OnSelectInventory?.AddListener(() =>
            {
                Inventory.SelectSlot();
            });

            OnDeselectInventory?.AddListener(() =>
            {
                Inventory.DeselectSlot();
            });

            // Tower
            // ActionMap 스위칭은 PlayerInputController 클래스 내부에서 수행합니다.
            Inventory.OnUseConstructiveItem?.AddListener((item) =>
            {
                // 인벤토리의 Visibility를 끄고
                Inventory.ToggleCanvas(false);
                _isTowerSpawnerOpen = true;

                // 타워 설치 모드로 진입합니다.
                Tower.EnterSpawnMode(item, transform.position);
            });

            OnExitSpawn?.AddListener(() =>
            {
                Tower.ExitSpawnMode();
                Inventory.ToggleCanvas(true);
                _isTowerSpawnerOpen = false;
            });

            // Spawn이 성공하거나 실패하면 인벤토리를 다시 켭니다
            OnCancelSpawn?.AddListener(() =>
            {
                Inventory.RestoreReservedSlot();
                OnExitSpawn?.Invoke();
            });
            OnConfirmSpawn?.AddListener(() =>
            {
                if (Tower.TrySpawnTower() == true)
                {
                    OnExitSpawn?.Invoke();
                }
            });
            OnMovePrefabSpawn?.AddListener((direction) =>
            {
                Tower.MoveTowerPreview(direction);
            });
        }

        public void SetState(PlayerState state, bool needParamters = false)
        {
            State.SetState(state, needParamters);
            if (state == PlayerState.Dodge)
            {
                _isPlayerDodging = true;
            }
            else
            {
                _isPlayerDodging = false;
            }
        }

        public void TakeDamage(IDamager damager, float damage)
        {
            if (_isPlayerDodging == true)
            {
                return;
            }

            if (damager?.Transform.gameObject.layer == LayerRepository.LAYER_BOSS_MONSTER)
            {
                Knockback(damager.Transform);
            }

            OnPlayerDamaged?.Invoke(damage);
        }

        public void Dead()
        {
            OnDead?.Invoke();
            _isDead = true;

            State.SetState(PlayerState.Idle); // MoveState 설정 
            State.SetState(PlayerState.Dead); // ActionState 설정
        }

        public void Knockback(Transform attackerTransform)
        {
            State.Parameters.AttackerTransform = attackerTransform;
            State.SetState(PlayerState.Idle);
            State.SetState(PlayerState.Knockback, true);
        }
    }
}