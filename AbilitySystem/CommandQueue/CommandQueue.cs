using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public sealed class CommandQueue : ICommandQueue
    {
        private readonly ICombatEventBus _combatEventBus;
        private readonly List<ICommand> _commands = new();
        private readonly TimeEvents _timeEvents;

        public int Time { get; private set; }
        

        public CommandQueue(ICombatEventBus combatEventBus)
        {
            _combatEventBus = combatEventBus;
            _timeEvents = new TimeEvents();
        }

        public void Add(ICommand command)
        {
            _commands.Add(command);
        }

        public void AddTimeEvent(int time, Action action)
        {
            _timeEvents.AddTimeEvent(time, action);
        }

        public ReadOnlyCollection<ICommand> CalcResult()
        {
            while (!_timeEvents.IsMaxTime(Time))
            {
                UpdateTime();
            }
            return _commands.AsReadOnly();
        }

        private void UpdateTime()
        {
            Time += 1;
            
           _timeEvents.UpdateTime(Time);
        }
    }
}