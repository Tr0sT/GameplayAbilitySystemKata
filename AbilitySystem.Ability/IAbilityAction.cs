namespace Nuclear.AbilitySystem
{
    public interface IAbilityAction
    {
        void Execute(IUnitId sourceId, IUnitId? targetId, 
            ICombatEventBus eventBus, IAbilityContextHolder abilityContextHolder);
        IAbilityAction DeepClone();
    }
}

