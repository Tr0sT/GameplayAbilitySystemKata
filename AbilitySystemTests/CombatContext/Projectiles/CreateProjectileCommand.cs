namespace AbilitySystem
{
    public record CreateProjectileCommand(IUnit Source, IUnit Target, int FlyingTime, int Time) : ICommand;
}