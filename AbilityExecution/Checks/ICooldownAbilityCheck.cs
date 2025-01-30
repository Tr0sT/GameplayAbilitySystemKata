namespace AbilitySystem
{
    public interface ICooldownAbilityCheck : IAbilityCheck
    {
        int GetCooldownTimer(ITimeAbilityContext context);
    }
    
    public interface ITimeAbilityContext : IAbilityContext
    {
        int Time { get; }
    }
}