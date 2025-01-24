using System;
using System.Collections.Generic;

namespace AbilitySystem
{
    public sealed class CombatContext : ICombatContext
    {
        private readonly CommandQueue _commandQueue;
        private readonly ICombatEventsContext _combatEventsContext;
        
        private readonly Dictionary<int, List<Action>> _onTimeEvents = new();

        public CombatContext(CommandQueue commandQueue, ICombatEventsContext combatEventsContext)
        {
            _commandQueue = commandQueue;
            _combatEventsContext = combatEventsContext;
            _combatEventsContext.SubscribeToTimeChange(OnTimeChange);
        }

        private void OnTimeChange(int time)
        {
            if (_onTimeEvents.TryGetValue(time, out var events))
            {
                foreach (var action in events)
                {
                    action();
                }
            }
        }

        public void CreateProjectile(IUnit source, IUnit target, int flyingTime, Action onEnd)
        {
            if (source.IsDead || target.IsDead)
                return;
            
            _commandQueue.Add(new CreateProjectileCommand(source, target, flyingTime, _commandQueue.Time));

            AddTimeEvent(_commandQueue.Time + flyingTime, onEnd);
        }


        private void AddTimeEvent(int time, Action action)
        {
            _commandQueue.AddMaxTime(time);
            _onTimeEvents.TryGetValue(time, out var events);
            if (events != null)
            {
                events.Add(action);
            }
            else
            {
                _onTimeEvents.Add(time, new() {action});
            }
        }

        public void Delay(int time, Action onEnd)
        {
            AddTimeEvent(_commandQueue.Time + time, onEnd);
        }
    }
}