using System;

namespace AbilitySystem
{
    public interface ICombatEventBus
    {
        void Subscribe<T>(Func<T, bool> func) where T : ICombatEvent;
        bool Raise<T>(T @event) where T : ICombatEvent;
        void Unsubscribe<T>(Func<T, bool> func) where T : ICombatEvent;
    }
}