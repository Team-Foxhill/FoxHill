using FoxHill.Controller;
using FoxHill.Core.Damage;
using FoxHill.Core.Pause;
using FoxHill.Player.Data;
using FoxHill.Player.Inventory;
using FoxHill.Player.Quest;
using FoxHill.Tower;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace FoxHill.Player
{
    /// <summary>
    /// 플레이어와 관련된 이벤트 및 데이터, 상태 등을 전반적으로 관리합니다.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerManager : CharacterControllerBase, IDamageable
    /// 플레이어와 직접적으로 관련된 클래스(ex. HPController, InputController)는 PlayerManager를 가지고 있고
    /// 플레이어와 연관된 외부 시스템 클래스(ex. Quest, Inventory)는 PlayerManager가 가지고 있는 형태입니다.
    {
        public event Action OnDead = null;

        [HideInInspector] public UnityEvent<float> OnPlayerDamaged;
        [HideInInspector] public UnityEvent OnPlayerDead;
        [HideInInspector] public UnityEvent OnSwitchSkill;
        [HideInInspector] public UnityEvent OnCastSkill;

        [HideInInspector] public UnityEvent OnOpenInventory;
        [HideInInspector] public UnityEvent OnCloseInventory;
        [HideInInspector] public UnityEvent<Vector2> OnSwitchSlotInventory;
        [HideInInspector] public UnityEvent OnSelectInventory;
        [HideInInspector] public UnityEvent OnDeselectInventory;

        [HideInInspector] public UnityEvent OnEnterSpawn;
        [HideInInspector] public UnityEvent<Vector2> OnMovePrefabSpawn;
        [HideInInspector] public UnityEvent OnConfirmSpawn;
        [HideInInspector] public UnityEvent OnCancelSpawn;

        public bool IsInventoryOpen => _isInventoryOpen;
        public bool IsPaused => _isPaused;
        public bool IsDead => _isDead;
        public bool IsTowerSpawnerOpen => _isTowerSpawnerOpen;

        public Vector2 Direction
        {
            get => _direction;
            set
            {
                if (value == Vector2.zero)
                    return;

                _direction = value;
            }
        }

        public PlayerStat Stat { get; private set; }
        public CharacterController CharacterController { get; private set; }
        public PlayerQuestManager Quest { get; private set; }
        public PlayerInventory Inventory { get; private set; }
        [field:SerializeField] public TowerManager Tower { get; private set; }

        public Transform Transform => gameObject.transform;

        [SerializeField] private PlayerData _data;
        [SerializeField] private Vector2 _direction = Vector2.right;

        private bool _isInventoryOpen = false;
        private bool _isTowerSpawnerOpen = false;


        protected override void Awake()
        {
            base.Awake();

            CharacterController = GetComponent<CharacterController>();

            Stat = new PlayerStat();
            Stat.InitializeStat(_data);

            Quest = new PlayerQuestManager();

            if (Inventory == null)
            {
                Inventory = FindFirstObjectByType<PlayerInventory>();
            }

            if(Tower == null)
            {
                Tower = FindFirstObjectByType<TowerManager>();
            }
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

            // Spawn이 성공하거나 실패하면 인벤토리를 다시 켭니다
            OnCancelSpawn?.AddListener(() =>
            {
                Tower.ExitSpawnMode();

                Inventory.ToggleCanvas(true);
                _isTowerSpawnerOpen = false;
            });
            OnConfirmSpawn?.AddListener(() =>
            {
                Tower.TrySpawnTower();
                Tower.ExitSpawnMode();

                Inventory.ToggleCanvas(true);
                _isTowerSpawnerOpen = false;
            });
            OnMovePrefabSpawn?.AddListener((direction) =>
            {
                Tower.MoveTowerPreview(direction);
            });
        }

        public void TakeDamage(float damage)
        {
            if (IsPaused == true)
                return;

            OnPlayerDamaged?.Invoke(damage);
        }

        public void Dead()
        {
            if (IsPaused == true)
                return;

            OnPlayerDead?.Invoke();
        }
    }
}