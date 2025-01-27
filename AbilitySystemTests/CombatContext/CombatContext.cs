using System;

namespace AbilitySystem
{
    public sealed class CombatContext : ICombatContext
    {
        private readonly CommandQueue _commandQueue;

        public CombatContext(CommandQueue commandQueue)
        {
            _commandQueue = commandQueue;
        }


        public void CreateProjectile(IUnit source, IUnit target, int flyingTime, Action onEnd)
        {
            if (source.IsDead || target.IsDead)
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