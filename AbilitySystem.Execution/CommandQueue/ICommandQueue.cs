using System;
using System.Collections.ObjectModel;

namespace Nuclear.AbilitySystem
{
    public interface ICommandQueue : IReadOnlyCommandQueue
    {
        void Add(ICombatCommand combatCommand);
        int Time { get; }
        void AddTimeEvent(int time, Action action);
        
    }

    public interface IReadOnlyCommandQueue
    {
        ReadOnlyCollection<ICombatCommand> CalculateCommandQueue();
    }
}