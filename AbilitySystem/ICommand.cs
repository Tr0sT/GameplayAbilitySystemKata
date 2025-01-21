namespace AbilitySystem
{
    public interface ICommand { }

    public record AttackCommand(IUnit Attacker, IUnit Target, int Damage) : ICommand;

    public record DeathCommand(IUnit Target) : ICommand;

    public record TryAttackCommand(IUnit Attacker, IUnit Target) : ICommand;
    
    public record DefendCommand(IUnit Defender, IUnit Target) : ICommand;
}