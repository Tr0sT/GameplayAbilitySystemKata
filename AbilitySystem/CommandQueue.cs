using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public sealed class CommandQueue : ICommandQueue
    {
        private readonly List<ICommand> _commands = new();

        public void Add(ICommand command)
        {
            // TODO?
        }
        
        public ReadOnlyCollection<ICommand> GetResult()
        {
            // TODO?
            return _commands.AsReadOnly();
        }
    }
}