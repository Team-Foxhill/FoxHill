using FoxHill.Core.Stat;
using FoxHill.Player.Stat;

namespace FoxHill.Player.Data
{
    public class PlayerStat : IPlayerStat
    {
        public PlayerStat(PlayerData data)
        {
            MaxHp = data.Hp;
            CurrentHp = data.Hp;
            MoveSpeed = data.MoveSpeed;
            Power = data.Power;
            Defense = data.Defense;
            AttackSpeed = data.AttackSpeed;
            Exp = 0f;
            Level = 1;
        }

        public float MaxHp { get; set; }
        public float CurrentHp { get; set; }
        public float MoveSpeed { get; set; }
        public float Power { get; set; }
        public float Defense { get; set; }
        public float AttackSpeed { get; set; }
        public float Exp { get; set; }
        public int Level { get; set; }
    }
}