namespace AbilitySystem
{
    public interface IStatusEffect
    {
        void Init(IUnit targetUnit, CommandQueue commandQueue, ICombatEventsContext combatEventsContext);
    }
}