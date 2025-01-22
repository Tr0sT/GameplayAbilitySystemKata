using System;
using System.Collections.Generic;

namespace AbilitySystem
{
    public sealed class CombatEventsContext : ICombatEventsContext
    {
        private readonly List<Func<IUnit, IUnit, int, bool>> _preDamageSubscribers = new();
        private readonly List<Func<IUnit, IUnit, int, bool>> _afterDamageSubscribers = new();
        
        public void SubscribeToPreDamage(Func<IUnit, IUnit, int, bool> func)
        {
            _preDamageSubscribers.Add(func);
        }

        public void SubscribeToAfterDamage(Func<IUnit, IUnit, int, bool> func)
        {
            _afterDamageSubscribers.Add(func);
        }

        public bool RaisePreDamage(IUnit source, IUnit target, int damage)
        {
            foreach (var preDamageSubscriber in _preDamageSubscribers)
            {
                var cancel = preDamageSubscriber.Invoke(source, target, damage);
                if (cancel)
                {
                    return true;
                }
            }

            return false;
        }

        public bool RaiseAfterDamage(IUnit source, IUnit target, int damage)
        {
            foreach (var afterDamageSubscriber in _afterDamageSubscribers)
            {
                var interrupt = afterDamageSubscriber.Invoke(source, target, damage);
                if (interrupt)
                {
                    return true;
                }
            }

            return false;
        }
    }
}