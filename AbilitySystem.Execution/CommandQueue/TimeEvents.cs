using System;
using System.Collections.Generic;

namespace Nuclear.AbilitySystem
{
    internal sealed class TimeEvents
    {
        private readonly Dictionary<int, List<Action>> _onTimeEvents = new();
        
        private float _maxTime;

        public void UpdateTime(int time)
        {
            if (_onTimeEvents.TryGetValue(time, out var events))
            {
                foreach (var action in events)
                {
                    action();
                }
            }
        }
        
        public void AddTimeEvent(int time, Action action)
        {
            AddMaxTime(time);
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

        private void AddMaxTime(float maxTime)
        {
            _maxTime = MathF.Max(_maxTime, maxTime);
        }
        
        public bool IsMaxTime(int time) => time >= _maxTime;
    }
}