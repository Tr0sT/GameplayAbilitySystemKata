namespace AbilitySystem
{
    public interface ICombatEvent { }
    public sealed record PreDamageEvent(IUnit Source, IUnit Target, int Damage) : ICombatEvent;
    public sealed record AfterDamageEvent(IUnit Source, IUnit Target, int Damage) : ICombatEvent;

    public sealed record TimeChangeEvent(int Time) : ICombatEvent;
}