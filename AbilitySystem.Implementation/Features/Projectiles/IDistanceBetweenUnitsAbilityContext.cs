namespace Nuclear.AbilitySystem
{
    public interface IDistanceBetweenUnitsAbilityContext : IAbilityContext
    {
        float GetDistanceBetween(IUnitId unitId1, IUnitId unitId2);
    }
}