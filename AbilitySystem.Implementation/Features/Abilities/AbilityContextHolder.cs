using System;
using System.Collections.Generic;

namespace AbilitySystem
{
    public sealed class AbilityContextHolder : IAbilityContextHolder
    {
        private readonly Dictionary<Type, IAbilityContext> _contexts = new();

        public AbilityContextHolder()
        {
            _contexts.Add(typeof(ITimeAbilityContext), new TimeAbilityContext(0));
        }
        
        private AbilityContextHolder(AbilityContextHolder abilityContextHolder)
        {
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