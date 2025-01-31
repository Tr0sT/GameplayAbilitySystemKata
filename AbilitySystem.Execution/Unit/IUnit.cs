namespace Nuclear.AbilitySystem
{
    public interface IUnit 
    {
        IUnitId Id { get; }
        T GetCombatFeature<T>() where T : ICombatFeature;
        IUnit DeepClone();
        void Subscribe(ICombatEventBus combatEventBus);
        void UnSubscribe();
    }

    public interface IUnitId
    {
        bool Equals(IUnitId anotherId);
    }
}