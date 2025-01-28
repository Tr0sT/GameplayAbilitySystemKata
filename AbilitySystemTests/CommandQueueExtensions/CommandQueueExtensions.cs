using System;

namespace AbilitySystem
{
    public sealed class CommandQueueExtensions : ICommandQueueExtensions
    {
        private readonly CommandQueue _commandQueue;
        private readonly ICombatEventBus _combatEventBus;

        public CommandQueueExtensions(CommandQueue commandQueue, ICombatEventBus combatEventBus)
        {
            _commandQueue = commandQueue;
            _combatEventBus = combatEventBus;
        }


        public void CreateProjectile(IUnitId sourceId, IUnitId targetId, int flyingTime, Action onEnd)
        {
            var source = _combatEventBus.GetUnit(sourceId);
            var target = _combatEventBus.GetUnit(targetId);
            if (!source.GetCombatFeature<IDamageable>().CanInteract || 
                !target.GetCombatFeature<IDamageable>().CanInteract)
                return;
            
            _commandQueue.Add(new CreateProjectileCommand(source.Id, target.Id, flyingTime, _commandQueue.Time));

            _commandQueue.AddTimeEvent(_commandQueue.Time + flyingTime, onEnd);
        }

        public void Delay(int time, Action onEnd)
        {
            _commandQueue.AddTimeEvent(_commandQueue.Time + time, onEnd);
        }
    }
}