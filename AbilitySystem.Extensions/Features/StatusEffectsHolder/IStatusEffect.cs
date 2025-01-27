namespace AbilitySystem
{
    public interface IStatusEffect
    {
        void Init(IUnit targetUnit, ICommandQueue commandQueue, ICombatEventsContext combatEventsContext);
    }
}