namespace Nuclear.AbilitySystem
{
    public record AttackCombatCommand(IUnitId AttackerId, IUnitId TargetId, int Damage, int Time) : ICombatCommand;
}