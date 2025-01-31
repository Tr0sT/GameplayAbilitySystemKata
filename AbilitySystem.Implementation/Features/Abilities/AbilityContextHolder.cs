using System;
using System.Collections.Generic;

namespace Nuclear.AbilitySystem
{
    public sealed class AbilityContextHolder : IAbilityContextHolder
    {
        private readonly ICombatEventBus _combatEventsContext;
        private readonly Dictionary<Type, IAbilityContext> _contexts = new();

        public AbilityContextHolder(ICombatEventBus combatEventsContext)
        {
            _combatEventsContext = combatEventsContext;
            _contexts.Add(typeof(ITimeAbilityContext), new TimeAbilityContext(0));
            _contexts.Add(typeof(IDistanceBetweenUnitsAbilityContext), new DistanceBetweenUnitsAbilityContext(_combatEventsContext));
        }
        
        private AbilityContextHolder(AbilityContextHolder abilityContextHolder)
        {
            _combatEventsContext = abilityContextHolder._combatEventsContext;
            foreach (var (type, context) in abilityContextHolder._contexts)
            {
                _contexts.Add(type, context.DeepClone());
            }
        }
        public T GetContext<T>() where T : IAbilityContext
        {
            return (T)_contexts[typeof(T)];
        }

        public IAbilityContextHolder DeepClone()
        {
            return new AbilityContextHolder(this);
        }
    }
}