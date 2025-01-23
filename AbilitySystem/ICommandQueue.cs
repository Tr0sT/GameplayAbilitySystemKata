using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public interface ICommandQueue
    {
        ReadOnlyCollection<ICommand> GetResult();
        void Add(ICommand command);
    }
}