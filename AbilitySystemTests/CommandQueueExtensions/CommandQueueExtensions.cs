using System;

namespace AbilitySystem
{
    public sealed class CommandQueueExtensions : ICommandQueueExtensions
    {
        private readonly ICombatEventBus _combatEventBus;

        public CommandQueueExtensions(ICombatEventBus combatEventBus)
        {
            _combatEventBus = combatEventBus;
        }


        public void CreateProjectile(IUnitId sourceId, IUnitId targetId, int flyingTime, Action onEnd)
        {
            var source = _combatEventBus.GetUnit(sourceId);
            var target = _combatEventBus.GetUnit(targetId);
            if (!source.GetCombatFeature<IDamageable>().CanInteract || 
                !target.GetCombatFeature<IDamageable>().CanInteract)
                return;
            
            _combatEventBus.CommandQueue.Add(new CreateProjectileCommand(source.Id, target.Id, flyingTime, _combatEventBus.CommandQueue.Time));

            _combatEventBus.CommandQueue.AddTimeEvent(_combatEventBus.CommandQueue.Time + flyingTime, onEnd);
        }

        public void Delay(int time, Action onEnd)
        {
            _combatEventBus.CommandQueue.AddTimeEvent(_combatEventBus.CommandQueue.Time + time, onEnd);
        }
    }
}