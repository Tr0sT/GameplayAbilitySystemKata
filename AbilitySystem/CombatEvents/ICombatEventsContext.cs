using System;

namespace AbilitySystem
{
    public interface ICombatEventsContext
    {
        void SubscribeToPreDamage(Func<IUnit, IUnit, int, bool> func);
        void SubscribeToAfterDamage(Func<IUnit, IUnit, int, bool> func);
        bool RaisePreDamage(IUnit source, IUnit target, int value);
        bool RaiseAfterDamage(IUnit source, IUnit target, int value);
        void SubscribeToTimeChange(Action<int> onTimeChanged);
        void RaiseTimeChange(int time);
    }
}