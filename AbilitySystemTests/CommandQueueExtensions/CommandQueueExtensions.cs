using System;

namespace AbilitySystem
{
    public sealed class CommandQueueExtensions : ICommandQueueExtensions
    {
        private readonly CommandQueue _commandQueue;

        public CommandQueueExtensions(CommandQueue commandQueue)
        {
            _commandQueue = commandQueue;
        }


        public void CreateProjectile(IUnit source, IUnit target, int flyingTime, Action onEnd)
        {
            if (!source.GetCombatFeature<IDamageable>().CanInteract || 
                !target.GetCombatFeature<IDamageable>().CanInteract)
                return;
            
            _commandQueue.Add(new CreateProjectileCommand(source, target, flyingTime, _commandQueue.Time));

            _commandQueue.AddTimeEvent(_commandQueue.Time + flyingTime, onEnd);
        }

        public void Delay(int time, Action onEnd)
        {
            _commandQueue.AddTimeEvent(_commandQueue.Time + time, onEnd);
        }
    }
}