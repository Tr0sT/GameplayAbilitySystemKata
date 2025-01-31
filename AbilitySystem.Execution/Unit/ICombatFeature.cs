namespace Nuclear.AbilitySystem
{
    public interface ICombatFeature
    {
        ICombatFeature DeepClone();
        void Subscribe(ICombatEventBus combatEventBus);
        void UnSubscribe();
    }
}