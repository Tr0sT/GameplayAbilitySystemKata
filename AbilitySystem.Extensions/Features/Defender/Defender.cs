namespace AbilitySystem
{
    public sealed class Defender : IStatusEffect
    {
        private readonly IUnit _unit;
        private readonly ICommandQueue _commandQueue;
        private readonly ICombatEventBus _combatEventBus;


        public Defender(IUnit unit, ICommandQueue commandQueue, ICombatEventBus combatEventBus)
        {
            _unit = unit;
            _commandQueue = commandQueue;
            _combatEventBus = combatEventBus;
            _combatEventBus.Subscribe<PreDamageEvent>(OnPreDamage);
        }

        public void Dispose()
        {
            _combatEventBus.Unsubscribe<PreDamageEvent>(OnPreDamage);
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