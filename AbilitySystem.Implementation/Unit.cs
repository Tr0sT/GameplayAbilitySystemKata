using System;
using System.Collections.Generic;

namespace Nuclear.AbilitySystem
{
    public abstract class Unit : IUnit
    {
        protected readonly Dictionary<Type, ICombatFeature> _features = new();
        protected ICombatEventBus? _combatEventBus;
        
        protected Unit(IUnitId id)
        {
            Id = id;
        }

        protected Unit(Unit unit)
        {
            Id = unit.Id;
            foreach (var (type, feature) in unit._features)
            {
                _features.Add(type, feature.DeepClone());
            }
        }

        public IUnitId Id { get; }

        public T GetCombatFeature<T>() where T : ICombatFeature
        {
            return (T)_features[typeof(T)];
        }

        public abstract IUnit DeepClone();

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
