namespace AbilitySystem
{
    public interface ICombatFeature
    {
        ICombatFeature DeepClone();
        void Subscribe(ICommandQueue commandQueue, ICombatEventBus combatEventBus);
        void UnSubscribe();
    }
}