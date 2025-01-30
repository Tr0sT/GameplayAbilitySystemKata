using System;

namespace AbilitySystem
{
    public interface IAbilityAction
    {
        void Execute(IUnitId source, IUnitId? target, ICombatEventBus context);
        IAbilityAction DeepClone();
    }
    
    public enum AbilityActionTarget
    {
        FromSourceToTarget,
        FromTargetToSource,
        FromTargetToTarget,
        FromSourceToSource
    }
    
    public static class AbilityActionTargetExtensions
    {
        public static void UpdateAbilityActionTarget(AbilityActionTarget skillActionTarget, 
            IUnitId? source, IUnitId? target,
            out IUnitId? effectSource, out IUnitId? effectTarget)
        {
            switch (skillActionTarget)
            {
                case AbilityActionTarget.FromSourceToTarget:
                    effectTarget = target;
                    effectSource = source;
                    return;
                case AbilityActionTarget.FromTargetToSource:
                    effectTarget = source;
                    effectSource = target;
                    return;
                case AbilityActionTarget.FromTargetToTarget:
                    effectTarget = target;
                    effectSource = target;
                    return;
                case AbilityActionTarget.FromSourceToSource:
                    effectTarget = source;
                    effectSource = source;
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

