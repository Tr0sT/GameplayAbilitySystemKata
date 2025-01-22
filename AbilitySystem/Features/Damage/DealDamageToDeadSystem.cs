using System.Collections.Generic;
using System.Linq;

namespace AbilitySystem.ECS
{
    public sealed class DealDamageToDeadCombatSystem : ICombatSystem
    {
        public bool Process(List<Unit> units, CommandQueue commandQueue)
        {
            var activeCommand = commandQueue.GetActiveCommand();
            if (activeCommand is AttackCommand {Target: {IsDead: true}} attackCommand && 
                commandQueue.GetTags(attackCommand).FirstOrDefault(t => t is DealDamageProcessedTag) == null)
            {
                commandQueue.Remove(activeCommand);
                return true;
            }

            return false;
        }
    }
}