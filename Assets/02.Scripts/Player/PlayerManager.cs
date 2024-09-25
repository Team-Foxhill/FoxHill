using FoxHill.Controller;
using FoxHill.Core.Damage;
using FoxHill.Core.Pause;
using FoxHill.Player.Data;
using FoxHill.Player.Inventory;
using FoxHill.Player.Quest;
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
    {
        public event Action OnDead;
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
        public bool IsSpawnOpen => _isSpawnOpen;

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
        public PrefabSpawner Spawner { get; private set; }

        [SerializeField] private PlayerData _data;
        [SerializeField] private Vector2 _direction = Vector2.right;

        private bool _isInventoryOpen = false;
        private bool _isSpawnOpen = false;


        protected override void Awake()
        {
            base.Awake();

            CharacterController = GetComponent<CharacterController>();

            Stat = new PlayerStat();
            Stat.InitializeStat(_data);

            Quest = new PlayerQuestManager();

            if (Inventory == null)
                Inventory = FindFirstObjectByType<PlayerInventory>();
            if (Spawner == null)
                Spawner = FindFirstObjectByType<PrefabSpawner>();
        }

        private void Start()
        {
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

            OnCancelSpawn?.AddListener(() =>
            {
                _isSpawnOpen = false;
            });
            OnConfirmSpawn?.AddListener(() =>
            {
                Spawner.ConfirmSpawn();
            });
            OnMovePrefabSpawn?.AddListener((input) =>
            {
                Spawner.OnMovePrefabSpawn(input);
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