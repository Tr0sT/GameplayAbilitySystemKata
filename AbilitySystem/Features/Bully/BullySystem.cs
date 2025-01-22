using System.Collections.Generic;
using System.Linq;

namespace AbilitySystem.ECS
{
    public sealed class BullyCombatSystem : ICombatSystem
    {
        private readonly List<AttackCommand> _checkedAttackCommand = new List<AttackCommand>();
        
        public bool Process(List<Unit> units, CommandQueue commandQueue)
        {
            var activeCommand = commandQueue.GetActiveCommand();
            if (activeCommand is AttackCommand attackCommand && 
                !_checkedAttackCommand.Any(c => ReferenceEquals(c, attackCommand)))
            {
                _checkedAttackCommand.Add(attackCommand);
                var bully = units.Find(u => u.IsBully() && 
                                               u != attackCommand.Attacker && 
                                               u != attackCommand.Target &&
                                               !u.IsDead);
                if (bully != null)
                {
                    commandQueue.InsertCommandAfterAnotherCommand(new AttackCommand(bully, attackCommand.Target, bully.Damage),
                        attackCommand);
                }
                return true;
            }

            return false;
        }
    }
}