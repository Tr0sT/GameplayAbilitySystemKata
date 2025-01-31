using System;
using System.Collections.Generic;
using System.Linq;

namespace Nuclear.AbilitySystem
{
    public sealed class CombatEventBus : ICombatEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _combatEvents = new();

        private readonly List<IUnit> _units;
        private readonly CommandQueue _commandQueue;

        public static ICombatEventBus DeepCloneAndCreate(List<IUnit> units)
        {
            var unitClones = new List<IUnit>(units.Count);
            unitClones.AddRange(units.Select(u => u.DeepClone()));
            return new CombatEventBus(unitClones);
        }
        
        private CombatEventBus(List<IUnit> units)
        {
            _commandQueue = new CommandQueue();
            _units = units;
            _units.ForEach(u => u.Subscribe(this));
        }

        public void Dispose()
        {
            _units.ForEach(u => u.UnSubscribe());
        }

        public ICommandQueue CommandQueue => _commandQueue;

        public IUnit GetUnit(IUnitId unitId)
        {
            return _units.First(u => u.Id.Equals(unitId));
        }

        public void Subscribe<T>(Func<T, bool> func) where T : ICombatEvent
        {
            if (_combatEvents.TryGetValue(typeof(T), out var list))
            {
                list.Add(func);
            }
            else
            {
                _combatEvents.Add(typeof(T), new List<Delegate>(){func});
            }
        }

        public bool Raise<T>(T @event) where T : ICombatEvent
        {
            if (_combatEvents.TryGetValue(typeof(T), out var list))
            {
                foreach (var subscriber in list)
                {
                    var interrupt = ((Func<T, bool>)subscriber).Invoke(@event);
                    if (interrupt)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Unsubscribe<T>(Func<T, bool> func) where T : ICombatEvent
        {
            if (_combatEvents.TryGetValue(typeof(T), out var list))
            {
                list.Remove(func);
                if (list.Count == 0)
                {
                    _combatEvents.Remove(typeof(T));
                }
            }
        }
    }
}