namespace Nuclear.AbilitySystem
{
    public record TryAttackCombatCommand(IUnitId AttackerId, IUnitId TargetId, int Time) : ICombatCommand;
}