namespace Nuclear.AbilitySystem
{
    public record DefendCommand(IUnitId DefenderId, IUnitId TargetId, int Time) : ICommand;
}