namespace AbilitySystem
{
    public interface IStatusEffect
    {
        IStatusEffect DeepClone();
        void Subscribe(ICommandQueue commandQueue, ICombatEventBus combatEventBus);
        void UnSubscribe();
    }
}