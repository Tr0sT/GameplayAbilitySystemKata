namespace AbilitySystem
{
    public record AttackCommand(IUnitId AttackerId, IUnitId TargetId, int Damage, int Time) : ICommand;
}