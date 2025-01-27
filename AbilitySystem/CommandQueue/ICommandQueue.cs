using System;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public interface ICommandQueue
    {
        ReadOnlyCollection<ICommand> CalcResult();
        void Add(ICommand command);
        void AddTimeEvent(int time, Action action);
        int Time { get; }
    }
}