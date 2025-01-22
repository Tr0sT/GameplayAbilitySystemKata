using System.Collections.Generic;
using System.Linq;

namespace AbilitySystem.ECS
{
    public sealed class DealDamageCombatSystem : ICombatSystem
    {
        private readonly List<AttackCommand> _checkedAttackCommand = new List<AttackCommand>();
        public bool Process(List<Unit> units, CommandQueue commandQueue)
        {
            var activeCommand = commandQueue.GetActiveCommand();
            if (activeCommand is AttackCommand attackCommand && 
                !_checkedAttackCommand.Any(c => ReferenceEquals(c, attackCommand)))
            {
                var realDamage = attackCommand.Attacker.DealDamage(attackCommand.Target);
                var newCommand = new AttackCommand(attackCommand.Attacker, attackCommand.Target, realDamage);
                commandQueue.UpdateCommand(attackCommand, newCommand);
                commandQueue.AddTag(newCommand, new DealDamageProcessedTag());
                if (attackCommand.Target.IsDead)
                {
                    commandQueue.InsertCommandAfterAnotherCommand(new DeathCommand(attackCommand.Target), newCommand);
                }
                _checkedAttackCommand.Add(newCommand);
                return true;
            }

            return false;
        }
    }
}