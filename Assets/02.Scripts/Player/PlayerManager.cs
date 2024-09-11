using FoxHill.Controller;
using FoxHill.Player.Data;
using UnityEngine;

namespace FoxHill.Player
{
    public class PlayerManager : CharacterControllerBase
    {
        public bool IsPaused => _isPaused;
        public PlayerStat Stat { get; private set; }

        public CharacterController CharacterController { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            CharacterController = GetComponent<CharacterController>();

            var data = ScriptableObject.CreateInstance<PlayerData>();

            Stat = new PlayerStat();
            Stat.LoadData(data);
        }
    }
}