using System;
using System.Collections.Generic;

namespace AbilitySystem
{
    public sealed class CombatEventsContext : ICombatEventsContext
    {
        private readonly Dictionary<Type, List<Delegate>> _combatEvents = new();

        public void SubscribeToCombatEvent<T>(Func<T, bool> func) where T : ICombatEvent
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

        public bool RaiseCombatEvent<T>(T @event) where T : ICombatEvent
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

        public void UnsubscribeFromCombatEvent<T>(Func<T, bool> func) where T : ICombatEvent
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