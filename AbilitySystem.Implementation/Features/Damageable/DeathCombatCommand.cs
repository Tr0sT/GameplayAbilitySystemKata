namespace Nuclear.AbilitySystem
{
    public record DeathCombatCommand(IUnitId TargetId, int Time) : ICombatCommand;
}