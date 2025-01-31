using System.Collections.Generic;

namespace Nuclear.AbilitySystem
{
    public sealed class Defender : IStatusEffect
    {
        private readonly IUnitId _unitId;
        private ICombatEventBus? _combatEventBus;


        public Defender(IUnitId unitId)
        {
            _unitId = unitId;
        }

        public void Subscribe(ICombatEventBus combatEventBus)
        {
            _combatEventBus = combatEventBus;
            _combatEventBus.Subscribe<PreDamageEvent, DamageEventResult>(OnPreDamage);
        }

        public void UnSubscribe()
        {
            if (_combatEventBus == null)
            {
                return;
            }
            _combatEventBus.Unsubscribe<PreDamageEvent, DamageEventResult>(OnPreDamage);
            _combatEventBus = null;
        }

        public IStatusEffect DeepClone()
        {
            return new Defender(_unitId);
        }

        private DamageEventResult? OnPreDamage(PreDamageEvent @event, DamageEventResult? previousResult)
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
            
            _combatEventBus.CommandQueue.Add(new TryAttackCommand(@event.Source.Id, @event.Target.Id, _combatEventBus.CommandQueue.Time));
            _combatEventBus.CommandQueue.Add(new DefendCommand(_unitId, @event.Target.Id, _combatEventBus.CommandQueue.Time));
            @event.Source.GetCombatFeature<IDamageable>().DealDamage(unit, 1);
            return new (false);
        }
    }
}