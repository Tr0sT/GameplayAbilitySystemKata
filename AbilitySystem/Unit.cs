using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public sealed class Unit : IUnit
    {
        private readonly List<IStatusEffect> _statusEffects = new();
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

        public int TakeDamage(int damage)
        {
            if (IsDead)
            {
                throw new System.Exception("Unit is already dead");
            }

            var realDamage = Math.Min(Health, damage);
            Health -= realDamage;
            return realDamage;
        }

        public int DealDamage(IUnit target)
        {
            return target.TakeDamage(Damage);
        }

        public ReadOnlyCollection<IStatusEffect> StatusEffects => _statusEffects.AsReadOnly();
        public void AddStatusEffect(IStatusEffect statusEffect)
        {
            _statusEffects.Add(statusEffect);
        }
    }
}
