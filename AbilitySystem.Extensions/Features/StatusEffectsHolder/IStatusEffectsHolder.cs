using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public interface IStatusEffectsHolder : ICombatFeature
    {
        ReadOnlyCollection<IStatusEffect> StatusEffects { get; }

        void AddStatusEffect(IStatusEffect statusEffect);

    }

    public sealed class StatusEffectsHolder : IStatusEffectsHolder
    {
        private readonly IUnit _unit;
        private readonly ICommandQueue _commandQueue;
        private readonly ICombatEventsContext _combatEventsContext;
        private readonly List<IStatusEffect> _statusEffects = new();
        public ReadOnlyCollection<IStatusEffect> StatusEffects => _statusEffects.AsReadOnly();

        public StatusEffectsHolder(IUnit unit, ICommandQueue commandQueue, ICombatEventsContext combatEventsContext)
        {
            _unit = unit;
            _commandQueue = commandQueue;
            _combatEventsContext = combatEventsContext;
        }
        
        public void AddStatusEffect(IStatusEffect statusEffect)
        {
            _statusEffects.Add(statusEffect);
            statusEffect.Init(_unit, _commandQueue, _combatEventsContext);
        }

    }
}