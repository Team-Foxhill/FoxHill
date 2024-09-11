using FoxHill.Controller;
using FoxHill.Player.Data;
using FoxHill.Player.HP;
using UnityEngine;
using UnityEngine.Events;

namespace FoxHill.Player
{
    public class PlayerManager : CharacterControllerBase
    {
        public UnityEvent OnPlayerDamaged;
        public UnityEvent OnPlayerDead;

        public bool IsPaused => _isPaused;
        public PlayerStat Stat { get; private set; }
        [SerializeField] private PlayerData _data;

        public CharacterController CharacterController { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            CharacterController = GetComponent<CharacterController>();

            Stat = new PlayerStat();
            Stat.LoadData(_data);
        }
    }
}