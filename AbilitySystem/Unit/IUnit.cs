namespace AbilitySystem
{
    public interface IUnit 
    {
        T GetCombatFeature<T>() where T : ICombatFeature;
    }
}