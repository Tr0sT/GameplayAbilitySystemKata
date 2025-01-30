namespace AbilitySystem
{
    public record TryAttackCommand(IUnitId AttackerId, IUnitId TargetId, int Time) : ICommand;
}