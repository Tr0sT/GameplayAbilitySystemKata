using System;

namespace Nuclear.AbilitySystem
{
    public sealed class CooldownAbilityCheck : ICooldownAbilityCheck
    {
        private readonly int _cooldown;
        private int _lastCastTime;

        public CooldownAbilityCheck(int cooldown, int? startValue = null)
        {
            _cooldown = cooldown;
            _lastCastTime = -startValue ?? -cooldown;
        }

        public bool CanExecute(IUnitId source, IUnitId? target, IAbilityContextHolder context)
        {
            var timeContext = context.GetContext<ITimeAbilityContext>();
            return GetCooldownTimer(timeContext) == 0;
        }

        public void Execute(IUnitId source, IUnitId? target, IAbilityContextHolder context)
        {
            var timeContext = context.GetContext<ITimeAbilityContext>();
            _lastCastTime = timeContext.Time;
        }

        public int GetCooldownTimer(ITimeAbilityContext timeContext)
        {
            return Math.Max(0, _lastCastTime + _cooldown + 1 - timeContext.Time);
        }

        public IAbilityCheck DeepClone()
        {
            var result = new CooldownAbilityCheck(_cooldown);
            result._lastCastTime = _lastCastTime;
            return result;
        }
    }
}