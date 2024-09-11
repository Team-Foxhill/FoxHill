namespace FoxHill.Core.Damage
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
        void Dead();
    }
}