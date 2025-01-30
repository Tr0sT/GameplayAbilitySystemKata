namespace AbilitySystem
{
    public record DeathCommand(IUnitId TargetId, int Time) : ICommand;
}