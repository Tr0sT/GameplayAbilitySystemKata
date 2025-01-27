namespace AbilitySystem
{
    public sealed class Defender : IStatusEffect
    {
        private IUnit _unit = null!;
        private CommandQueue _commandQueue = null!;


        public void Init(IUnit unit, CommandQueue commandQueue, ICombatEventsContext combatEventsContext)
        {
            _unit = unit;
            _commandQueue = commandQueue;
            combatEventsContext.SubscribeToCombatEvent<PreDamageEvent>(OnPreDamage);
        }

        private bool OnPreDamage(PreDamageEvent @event)
        {
            if (_unit == @event.Source || _unit == @event.Target)
            {
                return false;
            }

            _commandQueue.Add(new TryAttackCommand(@event.Source, @event.Target, _commandQueue.Time));
            _commandQueue.Add(new DefendCommand(_unit, @event.Target, _commandQueue.Time));
            @event.Source.GetCombatFeature<IDamageable>().DealDamage(_unit);
            return true;
        }
    }
}