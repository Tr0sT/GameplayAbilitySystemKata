using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public sealed class CommandQueue : ICommandQueue
    {
        private readonly ICombatEventsContext _combatEventsContext;
        private readonly List<ICommand> _commands = new();
        
        public int Time { get; private set; }
        private float _maxTime;

        public CommandQueue(ICombatEventsContext combatEventsContext)
        {
            _combatEventsContext = combatEventsContext;
        }

        public void Add(ICommand command)
        {
            _commands.Add(command);
        }
        
        public ReadOnlyCollection<ICommand> GetResult()
        {
            return _commands.AsReadOnly();
        }

        public void UpdateTime()
        {
            Time += 1;
            _combatEventsContext.RaiseCombatEvent(new TimeChangeEvent(Time));
        }

        public void AddMaxTime(float maxTime)
        {
            _maxTime = MathF.Max(_maxTime, maxTime);
        }
        
        public bool IsMaxTime => Time >= _maxTime;
    }
}