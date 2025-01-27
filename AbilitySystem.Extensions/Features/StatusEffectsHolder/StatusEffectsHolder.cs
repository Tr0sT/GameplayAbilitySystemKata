using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public sealed class StatusEffectsHolder : IStatusEffectsHolder
    {
        private readonly IUnit _unit;
        private readonly ICommandQueue _commandQueue;
        private readonly ICombatEventsContext _combatEventsContext;
        private readonly List<IStatusEffect> _statusEffects = new();

        public StatusEffectsHolder(IUnit unit, ICommandQueue commandQueue, ICombatEventsContext combatEventsContext)
        {
            _unit = unit;
            _commandQueue = commandQueue;
            _combatEventsContext = combatEventsContext;
        }

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