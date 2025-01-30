namespace AbilitySystem
{
    public interface IStatusEffect
    {
        IStatusEffect DeepClone();
        void Subscribe(ICombatEventBus combatEventBus);
        void UnSubscribe();
    }
}