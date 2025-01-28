using System;
using System.Collections.Generic;
using System.Linq;

namespace AbilitySystem
{
    public sealed class CombatEventBus : ICombatEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _combatEvents = new();

        private readonly List<IUnit> _units;
        public CombatEventBus(List<IUnit> units, ICommandQueue commandQueue)
        {
            _units = new List<IUnit>(units.Count);
            _units.AddRange(units.Select(u => u.DeepClone()));
            _units.ForEach(u => u.Subscribe(commandQueue, this));
        }

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