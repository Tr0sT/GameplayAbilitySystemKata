using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public interface IUnit 
    {
        string Name { get; }
        int Health { get; }
        bool IsDead { get; }
        int Damage { get; }
        int TakeDamage(int damage);
        int DealDamage(IUnit target);
        
        ReadOnlyCollection<IStatusEffect> StatusEffects { get; }

        void AddStatusEffect(IStatusEffect statusEffect);
    }
}