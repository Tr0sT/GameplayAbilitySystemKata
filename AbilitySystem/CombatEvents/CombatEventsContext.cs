using System;
using System.Collections.Generic;

namespace AbilitySystem
{
    public sealed class CombatEventsContext : ICombatEventsContext
    {
        private readonly Dictionary<Type, List<Func<ICombatEvent, bool>>> _combatEvents = new();

        public void SubscribeToCombatEvent<T>(Func<T, bool> func) where T : ICombatEvent
        {
            if (_combatEvents.TryGetValue(typeof(T), out var list))
            {
                list.Add(e => func((T)e));
            }
            else
            {
                _combatEvents.Add(typeof(T), new List<Func<ICombatEvent, bool>>(){e => func((T)e)});
            }
        }

        public bool RaiseCombatEvent<T>(T @event) where T : ICombatEvent
        {
            if (_combatEvents.TryGetValue(typeof(T), out var list))
            {
                foreach (var subscriber in list)
                {
                    var interrupt = subscriber.Invoke(@event);
                    if (interrupt)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}