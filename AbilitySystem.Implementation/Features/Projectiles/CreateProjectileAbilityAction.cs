using System.Collections.ObjectModel;
using System.Linq;

namespace Nuclear.AbilitySystem
{
    public record ProjectileName(string Value);
    public sealed class CreateProjectileAbilityAction : IAbilityAction
    {
        public CreateProjectileAbilityAction(ProjectileName projectileName, 
            ReadOnlyCollection<IAbilityAction>? onEnd = null)
        {
            ProjectileName = projectileName;
            OnEnd = onEnd;
        }

        public ProjectileName ProjectileName { get; }
        public ReadOnlyCollection<IAbilityAction>? OnEnd { get; }
        public void Execute(IUnitId sourceId, IUnitId? targetId, ICombatEventBus context)
        {
            var source = context.GetUnit(sourceId);
            var target = context.GetUnit(targetId!);
            
            if (!source.GetCombatFeature<IDamageable>().CanInteract || 
                !target.GetCombatFeature<IDamageable>().CanInteract)
                return;

            var flyingTime = 1; // Calculate flying time from context?
            context.CommandQueue.Add(new CreateProjectileCommand(sourceId, targetId!, flyingTime, context.CommandQueue.Time));

            if (OnEnd != null)
            {
                context.CommandQueue.AddTimeEvent(context.CommandQueue.Time + flyingTime, () =>
                {
                    foreach (var onEndAction in OnEnd)
                    {
                        onEndAction.Execute(sourceId, targetId, context);
                    }
                });
            }
        }

        public IAbilityAction DeepClone()
        {
            return new CreateProjectileAbilityAction(ProjectileName, 
                OnEnd?.Select(a => a.DeepClone()).ToList().AsReadOnly());
        }
    }
}