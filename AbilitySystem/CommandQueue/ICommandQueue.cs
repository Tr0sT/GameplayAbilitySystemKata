using System;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public interface ICommandQueue
    {
        void Add(ICommand command);
        int Time { get; }
        void AddTimeEvent(int time, Action action);
        ReadOnlyCollection<ICommand> CalcResult();
    }
}