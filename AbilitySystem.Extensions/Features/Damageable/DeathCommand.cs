namespace AbilitySystem
{
    public record DeathCommand(IUnit Target, int Time) : ICommand;
}