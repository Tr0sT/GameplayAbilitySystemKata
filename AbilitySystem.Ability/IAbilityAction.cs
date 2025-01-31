namespace Nuclear.AbilitySystem
{
    public interface IAbilityAction
    {
        void Execute(IUnitId source, IUnitId? target, ICombatEventBus context);
        IAbilityAction DeepClone();
    }
}

