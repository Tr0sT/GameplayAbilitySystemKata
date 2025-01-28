using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public sealed class StatusEffectsHolder : IStatusEffectsHolder
    {
        private readonly List<IStatusEffect> _statusEffects = new();
        private readonly IUnitId _unitId;
        
        private ICombatEventBus? _combatEventBus;
        private ICommandQueue? _commandQueue;

        public ReadOnlyCollection<IStatusEffect> StatusEffects => _statusEffects.AsReadOnly();

        public StatusEffectsHolder(IUnitId unitId)
        {
            _unitId = unitId;
        }

        public void Subscribe(ICommandQueue commandQueue, ICombatEventBus combatEventBus)
        {
            _combatEventBus = combatEventBus;
            _commandQueue = commandQueue;
            var unit = _combatEventBus.GetUnit(_unitId);
            foreach (var statusEffect in _statusEffects)
            {
                statusEffect.Subscribe(_commandQueue, _combatEventBus);
            }
        }

        public void UnSubscribe()
        {
            _combatEventBus = null;
            _commandQueue = null;
            foreach (var statusEffect in _statusEffects)
            {
                statusEffect.UnSubscribe();
            }
        }

        public void AddStatusEffect(IStatusEffect statusEffect)
        {
            _statusEffects.Add(statusEffect);
        }

        public void RemoveStatusEffect(IStatusEffect statusEffect)
        {
            _statusEffects.Remove(statusEffect);
            statusEffect.UnSubscribe();
        }

        public ICombatFeature DeepClone()
        {
            var result = new StatusEffectsHolder(_unitId);
            foreach (var statusEffect in _statusEffects)
            {
                result._statusEffects.Add(statusEffect.DeepClone());
            }
            return result;
        }
    }
}