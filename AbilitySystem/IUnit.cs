namespace AbilitySystem
{
    public interface IUnit
    {
        string Name { get; }
        int Health { get; }
        bool IsDead { get; }
        int Damage { get; }
        void TakeDamage(int damage);
        void DealDamage(IUnit target);

    }
}