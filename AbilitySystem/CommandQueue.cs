using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AbilitySystem
{
    public sealed class CommandQueue : ICommandQueue
    {
        private readonly List<ICommand> _commands = new();
        private readonly List<bool> _dirty = new();
        public bool IsDirty => _dirty.Any(d => d);
        private readonly Dictionary<ICommand, List<ITag>> _tags = new(ReferenceEqualityComparer.Instance); 

        public void Add(ICommand command)
        {
            _commands.Add(command);
            _dirty.Add(true);
            _tags.Add(command, new List<ITag>());
        }
        
        public ReadOnlyCollection<ICommand> GetResult()
        {
            // TODO?
            return _commands.AsReadOnly();
        }

        public ICommand? GetActiveCommand()
        {
            var index = _dirty.FindIndex(d => d);
            return index == -1 ? null : _commands[index];
        }

        public void Remove(ICommand command)
        {
            var index = _commands.FindIndex(c => ReferenceEquals(c, command));
            _dirty.RemoveAt(index);
            _commands.RemoveAt(index);
            _tags.Remove(command);
        }

        public void SetActiveCommandProcessed()
        {
            var index = _dirty.FindIndex(d => d);
            if (index == -1)
            {
                return;
            }
            _dirty[index] = false;
        }

        public void UpdateCommand(ICommand oldCommand, ICommand newCommand)
        {
            var index = _commands.FindIndex(c => ReferenceEquals(c, oldCommand));
            _commands[index] = newCommand;
            _tags.Add(newCommand, _tags[oldCommand]);
            _tags.Remove(oldCommand);
        }

        public void InsertCommandBeforeAnotherCommand(ICommand insertCommand, ICommand anotherCommand)
        {
            var index = _commands.FindIndex(c => ReferenceEquals(c, anotherCommand));
            if (index == -1)
            {
                return;
            }
            
            _commands.Insert(index, insertCommand);
            _dirty.Insert(index, true);
            _tags.Add(insertCommand, new List<ITag>());
        }

        public void InsertCommandAfterAnotherCommand(ICommand insertCommand, ICommand anotherCommand)
        {
            var index = _commands.FindIndex(c => ReferenceEquals(c, anotherCommand));
            if (index == -1)
            {
                return;
            }
            
            _commands.Insert(index + 1, insertCommand);
            _dirty.Insert(index + 1, true);
            _tags.Add(insertCommand, new List<ITag>());
        }

        public ReadOnlyCollection<ITag> GetTags(ICommand command) => _tags[command].AsReadOnly();
        public void AddTag(ICommand command, ITag tag) => _tags[command].Add(tag);
    }
}