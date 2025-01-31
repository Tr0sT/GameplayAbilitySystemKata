using System;
using System.Collections.Generic;

namespace Nuclear.AbilitySystem
{
    public sealed class Unit : IUnit
    {
        private readonly Dictionary<Type, ICombatFeature> _features = new();
        private ICombatEventBus? _combatEventBus;
        public string Name { get; }
        public int Health { get; private set; }
        public int Damage { get; private set; }
        public bool IsDead => Health <= 0;
        
        public Unit(string name, int health, int damage)
        {
            Name = name;
            Health = health;
            Damage = damage;
            
            _features.Add(typeof(IDamageable), new Damageable(Id, u =>
                {
                    var unit = (Unit) u;
                    return !unit.IsDead;
                },
                (u, dmg) =>
                {
                    var unit = (Unit) u;
                    var realDamage = Math.Min(unit.Health, dmg);
                    unit.Health -= realDamage;
                    return realDamage;
                },
                (u) =>
                {
                    var unit = (Unit) u;
                    return unit.Damage;
                }));
            _features.Add(typeof(IStatusEffectsHolder), new StatusEffectsHolder(Id));
            _features.Add(typeof(IAbilitiesHolder), new AbilitiesHolder());
        }

        private Unit(Unit unit)
        {
            Name = unit.Name;
            Health = unit.Health;
            Damage = unit.Damage;
            
            foreach (var (type, feature) in unit._features)
            {
                _features.Add(type, feature.DeepClone());
            }
        }

        public IUnitId Id => new UnitId(Name);

        public T GetCombatFeature<T>() where T : ICombatFeature
        {
            return (T)_features[typeof(T)];
        }

        public IUnit DeepClone()
        {
            return new Unit(this);
        }

        public void Subscribe(ICombatEventBus combatEventBus)
        {
            _combatEventBus = combatEventBus;

            foreach (var combatFeature in _features)
            {
                combatFeature.Value.Subscribe(combatEventBus);
            }
        }

        public void UnSubscribe()
        {
            _combatEventBus = null;
            
            foreach (var combatFeature in _features)
            {
                combatFeature.Value.UnSubscribe();
            }
        }
    }
}
