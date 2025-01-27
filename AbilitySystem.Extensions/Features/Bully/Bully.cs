namespace AbilitySystem
{
    public sealed class Bully : IStatusEffect
    {
        private readonly IUnit _unit;
        private readonly ICombatEventBus _combatEventBus;

        public Bully(IUnit unit, ICombatEventBus combatEventBus)
        {
            _unit = unit;
            _combatEventBus = combatEventBus;

            _combatEventBus.Subscribe<AfterDamageEvent>(OnAfterDamage);
        }

        public void Dispose()
        {
            _combatEventBus.Unsubscribe<AfterDamageEvent>(OnAfterDamage);
        }

        private bool OnAfterDamage(AfterDamageEvent @event)
        {
            if (_unit == @event.Source || _unit == @event.Target)
            {
                return false;
            }

            _unit.GetCombatFeature<IDamageable>().DealDamage(@event.Target);
            return true;
        }
    }
}