namespace AbilitySystem
{
    public interface ICommand
    {
        public int Time { get; }
    }

    public record AttackCommand(IUnit Attacker, IUnit Target, int Damage, int Time) : ICommand;

    public record DeathCommand(IUnit Target, int Time) : ICommand;

    public record TryAttackCommand(IUnit Attacker, IUnit Target, int Time) : ICommand;
    
    public record DefendCommand(IUnit Defender, IUnit Target, int Time) : ICommand;

    public record CreateProjectileCommand(IUnit Source, IUnit Target, int FlyingTime, int Time) : ICommand;
}