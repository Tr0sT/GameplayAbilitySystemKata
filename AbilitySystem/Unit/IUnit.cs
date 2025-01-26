namespace AbilitySystem
{
    public interface IUnit 
    {
        string Name { get; }
        int Health { get; }
        bool IsDead { get; }
        
                
        T GetCombatFeature<T>() where T : ICombatFeature;
    }
}