namespace AbilitySystem
{
    public sealed class Bully : IStatusEffect
    {
        private IUnit _unit = null!;

        public void Init(IUnit unit, CommandQueue commandQueue, ICombatEventsContext combatEventsContext)
        {
            _unit = unit;
            
            combatEventsContext.SubscribeToAfterDamage(OnAfterDamage);
        }

        private bool OnAfterDamage(IUnit source, IUnit target, int damage)
        {
            if (_unit == source || _unit == target)
            {
                return false;
            }

            _unit.DealDamage(target);
            return true;
        }
    }
}