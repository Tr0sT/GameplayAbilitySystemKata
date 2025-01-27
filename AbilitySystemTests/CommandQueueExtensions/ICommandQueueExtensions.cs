using System;

namespace AbilitySystem
{
    public interface ICommandQueueExtensions
    {
        void CreateProjectile(IUnit source, IUnit target, int flyingTime, Action onHit);
        public void Delay(int time, Action onEnd);
    }
}