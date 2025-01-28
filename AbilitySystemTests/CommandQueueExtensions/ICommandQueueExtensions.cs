using System;

namespace AbilitySystem
{
    public interface ICommandQueueExtensions
    {
        void CreateProjectile(IUnitId sourceId, IUnitId targetId, int flyingTime, Action onHit);
        public void Delay(int time, Action onEnd);
    }
}