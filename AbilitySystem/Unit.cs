using System;

namespace AbilitySystem
{
    public sealed class Unit : IUnit
    {
        public string Name { get; }
        public int Health { get; private set; }
        public int Damage { get; }
        public bool IsDead => Health == 0;

        public Unit(string name, int health, int damage)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("A unit needs a name.", nameof(name));
            if (health < 0)
                throw new ArgumentOutOfRangeException(nameof(health));
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage));

            Name = name;
            Health = health;
            Damage = damage;
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage));

            Health -= Math.Min(Health, damage);
        }

        public void DealDamage(IUnit target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (!IsDead)
                target.TakeDamage(Damage);
        }
    }
}
