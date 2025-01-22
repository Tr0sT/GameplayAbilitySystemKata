using System.Collections.Generic;
using System.Linq;

namespace AbilitySystem.ECS
{
    public sealed class DefendCombatSystem : ICombatSystem
    {
        private readonly List<AttackCommand> _checkedAttackCommand = new List<AttackCommand>();
        
        public bool Process(List<Unit> units, CommandQueue commandQueue)
        {
            var activeCommand = commandQueue.GetActiveCommand();
            if (activeCommand is AttackCommand attackCommand && !attackCommand.Target.IsDefender() &&
                !_checkedAttackCommand.Any(c => ReferenceEquals(c, attackCommand)))
            {
                _checkedAttackCommand.Add(attackCommand);
                var defender = units.Find(u => u.IsDefender() && 
                                               u != attackCommand.Attacker &&
                                               !u.IsDead);
                if (defender != null)
                {
                    commandQueue.InsertCommandBeforeAnotherCommand(new TryAttackCommand(attackCommand.Attacker, attackCommand.Target), attackCommand);
                    commandQueue.InsertCommandBeforeAnotherCommand(new DefendCommand(defender, attackCommand.Target), attackCommand);
                    var newAttackCommand = new AttackCommand(attackCommand.Attacker, defender, attackCommand.Damage);
                    commandQueue.UpdateCommand(attackCommand, newAttackCommand);
                    _checkedAttackCommand.Add(newAttackCommand);
                }
                return true;
            }

            return false;
        }
    }
}