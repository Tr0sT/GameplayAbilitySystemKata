using System.Collections.Generic;

namespace Nuclear.AbilitySystem
{
    public sealed class Bully : IStatusEffect
    {
        private readonly IUnitId _unitId;
        private ICombatEventBus? _combatEventBus;

        public Bully(IUnitId unitId)
        {
            _unitId = unitId;
        }
        
        public void Subscribe(ICombatEventBus combatEventBus)
        {
            _combatEventBus = combatEventBus;
            _combatEventBus.Subscribe<AfterDamageEvent, DamageEventResult>(OnAfterDamage);
        }

        public void UnSubscribe()
        {
            if (_combatEventBus == null)
            {
                return;
            }
            _combatEventBus.Unsubscribe<AfterDamageEvent, DamageEventResult>(OnAfterDamage);
            _combatEventBus = null;
        }

        public IStatusEffect DeepClone()
        {
            return new Bully(_unitId);
        }

        private DamageEventResult? OnAfterDamage(AfterDamageEvent @event, DamageEventResult? previousResult)
        {
            if (previousResult is {ContinueExecution: false})
            {
                return previousResult;
            }
            if (_combatEventBus == null)
            {
                throw new();
            }
            
            if (EqualityComparer<IUnitId>.Default.Equals(_unitId, @event.Source.Id) ||
                EqualityComparer<IUnitId>.Default.Equals(_unitId, @event.Target.Id))
            {
                return previousResult ?? new (true);
            }

            var unit = _combatEventBus.GetUnit(_unitId);
            
            unit.GetCombatFeature<IDamageable>().DealDamage(@event.Target, 1);
            return new(false);
        }
    }
}