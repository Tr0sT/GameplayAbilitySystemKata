namespace AbilitySystem
{
    public interface ICooldownAbilityCheck : IAbilityCheck
    {
        int GetCooldownTimer(ITimeAbilityContext context);
    }
}