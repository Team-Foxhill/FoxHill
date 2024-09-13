using FoxHill.Controller;
using FoxHill.Core.Damage;
using FoxHill.Player.Data;
using FoxHill.Player.Quest;
using FoxHill.Player.Skill;
using UnityEngine;
using UnityEngine.Events;

namespace FoxHill.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerManager : CharacterControllerBase, IDamageable
    {
        [HideInInspector] public UnityEvent<float> OnPlayerDamaged;
        [HideInInspector] public UnityEvent OnPlayerDead;
        [HideInInspector] public UnityEvent OnSwitchSkill;
        [HideInInspector] public UnityEvent OnCastSkill;

        public bool IsPaused => _isPaused;
        public bool IsDead => _isDead;

        public PlayerStat Stat { get; private set; }
        [SerializeField] private PlayerData _data;

        public CharacterController CharacterController { get; private set; }
        public PlayerQuestManager Quest { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            CharacterController = GetComponent<CharacterController>();

            Stat = new PlayerStat();
            Stat.InitializeStat(_data);

            Quest = new PlayerQuestManager();
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