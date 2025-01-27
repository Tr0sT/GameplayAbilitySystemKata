namespace AbilitySystem
{
    public sealed class Bully : IStatusEffect
    {
        private readonly IUnit _unit;
        private readonly ICombatEventsContext _combatEventsContext;

        public Bully(IUnit unit, ICombatEventsContext combatEventsContext)
        {
            _unit = unit;
            _combatEventsContext = combatEventsContext;

            _combatEventsContext.SubscribeToCombatEvent<AfterDamageEvent>(OnAfterDamage);
        }

        public void Dispose()
        {
            _combatEventsContext.UnsubscribeFromCombatEvent<AfterDamageEvent>(OnAfterDamage);
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