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
            _combatEventBus.Subscribe<AfterDamageEvent>(OnAfterDamage);
        }

        public void UnSubscribe()
        {
            if (_combatEventBus == null)
            {
                return;
            }
            _combatEventBus.Unsubscribe<AfterDamageEvent>(OnAfterDamage);
            _combatEventBus = null;
        }

        public IStatusEffect DeepClone()
        {
            return new Bully(_unitId);
        }

        private bool OnAfterDamage(AfterDamageEvent @event)
        {
            if (_combatEventBus == null)
            {
                throw new();
            }
            
            if (_unitId.Equals(@event.Source.Id) || _unitId.Equals(@event.Target.Id))
            {
                return false;
            }

            var unit = _combatEventBus.GetUnit(_unitId);
            
            unit.GetCombatFeature<IDamageable>().DealDamage(@event.Target, 1);
            return true;
        }
    }
}