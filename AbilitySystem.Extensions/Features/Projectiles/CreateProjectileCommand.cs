namespace AbilitySystem
{
    public record CreateProjectileCommand(IUnitId SourceId, IUnitId TargetId, int FlyingTime, int Time) : ICommand;
}