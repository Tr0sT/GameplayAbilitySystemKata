namespace AbilitySystem
{
    public sealed class Bully : IStatusEffect
    {
        private IUnit _unit = null!;

        public void Init(IUnit unit, ICommandQueue commandQueue, ICombatEventsContext combatEventsContext)
        {
            _unit = unit;
            
            combatEventsContext.SubscribeToCombatEvent<AfterDamageEvent>(OnAfterDamage);
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