namespace Nuclear.AbilitySystem
{
    public record DefendCombatCommand(IUnitId DefenderId, IUnitId TargetId, int Time) : ICombatCommand;
}