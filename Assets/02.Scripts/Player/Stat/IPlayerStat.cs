using FoxHill.Core.Stat;

namespace FoxHill.Player.Stat
{
    public interface IPlayerStat : IStat
    {
        float AttackSpeed { get; }
        float Exp { get; }
        int Level { get; }
    }
}