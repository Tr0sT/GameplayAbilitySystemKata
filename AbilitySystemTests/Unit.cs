using System;
using System.Collections.Generic;

namespace AbilitySystem
{
    public sealed class Unit : IUnit
    {
        private readonly CommandQueue _commandQueue;
        private readonly ICombatEventsContext _combatEventsContext;

        private readonly Dictionary<Type, ICombatFeature> _features = new();
        public string Name { get; }
        public int Health { get; private set; }
        public int Damage { get; private set; }
        public bool IsDead => Health <= 0;
        
        public Unit(string name, int health, int damage, 
            CommandQueue commandQueue, ICombatEventsContext combatEventsContext)
        {
            _commandQueue = commandQueue!;
            _combatEventsContext = combatEventsContext;
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
                _combatEventsContext, _commandQueue, this);
            _features.Add(typeof(IDamageable), damageable);

            var statusEffectHolder = new StatusEffectsHolder(this, commandQueue, _combatEventsContext);
            _features.Add(typeof(IStatusEffectsHolder), statusEffectHolder);
        }

        public T GetCombatFeature<T>() where T : ICombatFeature
        {
            return (T)_features[typeof(T)];
        }
    }
}
