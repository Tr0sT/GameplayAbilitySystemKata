using System;

namespace Nuclear.AbilitySystem
{
    public interface ICombatEventBus : IDisposable
    {
        public ICommandQueue CommandQueue { get; }
        IUnit GetUnit(IUnitId unitId);
        
        void Subscribe<T>(Func<T, bool> func) where T : ICombatEvent;
        bool Raise<T>(T @event) where T : ICombatEvent;
        void Unsubscribe<T>(Func<T, bool> func) where T : ICombatEvent;
    }
}