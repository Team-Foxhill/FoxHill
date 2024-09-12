namespace FoxHill.Core.Stat
{
    public interface IStat
    {
        float MaxHp { get; }
        float CurrentHp { get; }
        float MoveSpeed { get; }
        float Power { get; }
        float Defense { get; }
    }
}