namespace AbilitySystem
{
    public sealed class Unit : IUnit
    {
        public string Name { get; }
        public int Health { get; private set; }
        public int Damage { get; private set; }
        public bool IsDead => Health <= 0;
        
        public Unit(string name, int health, int damage)
        {
            Name = name;
            Health = health;
            Damage = damage;
        }

        public void TakeDamage(int damage)
        {
            if (IsDead) return;
            Health -= damage;
        }

        public void DealDamage(IUnit target)
        {
            target.TakeDamage(Damage);
        }

        public void ReactOnDamageToAnotherUnit_AddDamage(CommandQueue commandQueue)
        {
            // TODO
        }
        
        public void ReactOnDamageToAnotherUnit_Defend(CommandQueue commandQueue)
        {
            // TODO
        }
    }
}
