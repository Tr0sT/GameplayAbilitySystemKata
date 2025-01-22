using System.Collections.Generic;

namespace AbilitySystem.ECS
{
    public interface ICombatSystem
    {
        bool Process(List<Unit> units, CommandQueue commandQueue);
    }
}