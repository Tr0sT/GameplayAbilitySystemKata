using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public sealed class StatusEffectsHolder : IStatusEffectsHolder
    {
        private readonly List<IStatusEffect> _statusEffects = new();

        public ReadOnlyCollection<IStatusEffect> StatusEffects => _statusEffects.AsReadOnly();

        public void AddStatusEffect(IStatusEffect statusEffect)
        {
            _statusEffects.Add(statusEffect);
        }

        public void RemoveStatusEffect(IStatusEffect statusEffect)
        {
            _statusEffects.Remove(statusEffect);
            statusEffect.Dispose();
        }
    }
}