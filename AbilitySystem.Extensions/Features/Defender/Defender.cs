namespace AbilitySystem
{
    public sealed class Defender : IStatusEffect
    {
        private readonly IUnitId _unitId;
        private ICommandQueue? _commandQueue;
        private ICombatEventBus? _combatEventBus;


        public Defender(IUnitId unitId)
        {
            _unitId = unitId;
        }

        public void Subscribe(ICommandQueue commandQueue, ICombatEventBus combatEventBus)
        {
            _commandQueue = commandQueue;
            _combatEventBus = combatEventBus;
            _combatEventBus.Subscribe<PreDamageEvent>(OnPreDamage);
        }

        public void UnSubscribe()
        {
            if (_combatEventBus == null)
            {
                return;
            }
            _combatEventBus.Unsubscribe<PreDamageEvent>(OnPreDamage);
            _commandQueue = null;
            _combatEventBus = null;
        }

        public IStatusEffect DeepClone()
        {
            return new Defender(_unitId);
        }

        private bool OnPreDamage(PreDamageEvent @event)
        {
            if (_combatEventBus == null || _commandQueue == null)
            {
                throw new();
            }
            if (_unitId.Equals(@event.Source.Id) || _unitId.Equals(@event.Target.Id))
            {
                return false;
            }

            var unit = _combatEventBus.GetUnit(_unitId);
            
            _commandQueue.Add(new TryAttackCommand(@event.Source.Id, @event.Target.Id, _commandQueue.Time));
            _commandQueue.Add(new DefendCommand(_unitId, @event.Target.Id, _commandQueue.Time));
            @event.Source.GetCombatFeature<IDamageable>().DealDamage(unit);
            return true;
        }
    }
}