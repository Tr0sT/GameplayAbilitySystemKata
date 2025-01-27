namespace AbilitySystem
{
    public record DefendCommand(IUnit Defender, IUnit Target, int Time) : ICommand;
}