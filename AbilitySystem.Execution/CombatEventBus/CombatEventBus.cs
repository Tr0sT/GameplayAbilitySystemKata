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
            return _units.First(u => EqualityComparer<IUnitId>.Default.Equals(u.Id, unitId)); 
        }

        public void Subscribe<TEvent, TResult>(Func<TEvent, TResult?, TResult?> func) 
            where TEvent : ICombatEvent
            where TResult : ICombatEventResult
        {
            if (_combatEvents.TryGetValue(typeof(TEvent), out var list))
            {
                list.Add(func);
            }
            else
            {
                _combatEvents.Add(typeof(TEvent), new List<Delegate>(){func});
            }
        }

        public TResult? Raise<TEvent, TResult>(TEvent @event) 
            where TEvent : ICombatEvent
            where TResult : ICombatEventResult
        {
            var result = default(TResult);
            if (_combatEvents.TryGetValue(typeof(TEvent), out var list))
            {
                foreach (var subscriber in list)
                {
                    result = ((Func<TEvent, TResult?, TResult?>)subscriber).Invoke(@event, result);
                }
            }

            return result;
        }

        public void Unsubscribe<TEvent, TResult>(Func<TEvent, TResult?, TResult?> func) 
            where TEvent : ICombatEvent
            where TResult : ICombatEventResult
        {
            if (_combatEvents.TryGetValue(typeof(TEvent), out var list))
            {
                list.Remove(func);
                if (list.Count == 0)
                {
                    _combatEvents.Remove(typeof(TEvent));
                }
            }
        }
    }
}