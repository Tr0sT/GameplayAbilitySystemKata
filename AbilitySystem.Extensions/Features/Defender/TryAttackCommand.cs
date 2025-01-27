namespace AbilitySystem
{
    public record TryAttackCommand(IUnit Attacker, IUnit Target, int Time) : ICommand;
}