namespace FoxHill.Core.Damage
{
    public interface IStaggerable
    {
        bool IsFatalAttackable { get; }
        void MakeStun(IDamager damager);
    }
}