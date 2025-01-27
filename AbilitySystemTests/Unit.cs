using System;
using System.Collections.Generic;

namespace AbilitySystem
{
    public sealed class Unit : IUnit
    {
        private readonly ICommandQueue _commandQueue;
        private readonly ICombatEventBus _combatEventBus;

        private readonly Dictionary<Type, ICombatFeature> _features = new();
        public string Name { get; }
        public int Health { get; private set; }
        public int Damage { get; private set; }
        public bool IsDead => Health <= 0;
        
        public Unit(string name, int health, int damage, 
            ICommandQueue commandQueue, ICombatEventBus combatEventBus)
        {
            _commandQueue = commandQueue!;
            _combatEventBus = combatEventBus;
            Name = name;
            Health = health;
            Damage = damage;

            var damageable = new Damageable(() => !IsDead,
                dmg =>
                {
                    var realDamage = Math.Min(Health, dmg);
                    Health -= realDamage;
                    return realDamage;
                },
                () => Damage,
                _combatEventBus, _commandQueue, this);
            _features.Add(typeof(IDamageable), damageable);

            _features.Add(typeof(IStatusEffectsHolder), new StatusEffectsHolder());
        }

        public T GetCombatFeature<T>() where T : ICombatFeature
        {
            return (T)_features[typeof(T)];
        }
    }
}
