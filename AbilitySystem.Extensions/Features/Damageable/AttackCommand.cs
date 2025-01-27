namespace AbilitySystem
{
    public record AttackCommand(IUnit Attacker, IUnit Target, int Damage, int Time) : ICommand;
}