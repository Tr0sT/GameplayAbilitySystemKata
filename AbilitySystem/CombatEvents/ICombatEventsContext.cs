using System;

namespace AbilitySystem
{
    public interface ICombatEventsContext
    {
        void SubscribeToCombatEvent<T>(Func<T, bool> func) where T : ICombatEvent;
        bool RaiseCombatEvent<T>(T @event) where T : ICombatEvent;
    }
}