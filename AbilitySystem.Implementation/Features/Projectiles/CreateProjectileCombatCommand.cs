namespace Nuclear.AbilitySystem
{
    public record CreateProjectileCombatCommand(IUnitId SourceId, IUnitId TargetId, int FlyingTime, int Time) : ICombatCommand;
}