using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public sealed class CommandQueue : ICommandQueue
    {
        private readonly List<ICommand> _commands = new();

        public void Add(ICommand command)
        {
            _commands.Add(command);
        }
        
        public ReadOnlyCollection<ICommand> GetResult()
        {
            return _commands.AsReadOnly();
        }
    }
}