namespace AbilitySystem
{
    public interface IDamageable : ICombatFeature
    {
        bool CanInteract { get; }
        int TakeDamage(int damage);
        int DealDamage(IUnit target);
        IUnitId UnitId { get; }
    }
}