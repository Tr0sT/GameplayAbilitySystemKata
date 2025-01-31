using System.Collections.ObjectModel;
using System.Linq;

namespace Nuclear.AbilitySystem
{
    public sealed class DelayAbilityAction : IAbilityAction
    {
        public DelayAbilityAction(int time, 
            ReadOnlyCollection<IAbilityAction> onEnd)
        {
            Time = time;
            OnEnd = onEnd;
        }

        public int Time { get; }
        public ReadOnlyCollection<IAbilityAction> OnEnd { get; }
        public void Execute(IUnitId sourceId, IUnitId? targetId, ICombatEventBus context)
        {

            context.CommandQueue.AddTimeEvent(context.CommandQueue.Time + Time, () =>
            {
                foreach (var onEndAction in OnEnd)
                {
                    onEndAction.Execute(sourceId, targetId, context);
                }
            });
        }

        public IAbilityAction DeepClone()
        {
            return new DelayAbilityAction(Time, 
                OnEnd.Select(a => a.DeepClone()).ToList().AsReadOnly());
        }
    }
}