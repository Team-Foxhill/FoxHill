namespace FoxHill.Core.Debuff
{
    public enum DebuffType
    { 
        Slow
    }

    public interface IDebuffable
    {
        void ApplyDebuff(DebuffType type);
        void RemoveDebuff(DebuffType type);
    }
}