using System;

namespace AbilitySystem
{
    public sealed class Damageable : IDamageable
    {
        private readonly Func<bool> _canInteract;
        private readonly Func<int, int> _doDamage;
        private readonly Func<int> _getDamage;
        private readonly ICombatEventBus _combatEventBus;
        private readonly ICommandQueue _commandQueue;
        private readonly IUnit _unit;

        public IUnit Unit => _unit;

        public Damageable(Func<bool> canInteract,
            Func<int, int> doDamage,
            Func<int> getDamage,
            ICombatEventBus combatEventBus,
            ICommandQueue commandQueue,
            IUnit unit)
        {
            _canInteract = canInteract;
            _doDamage = doDamage;
            _getDamage = getDamage;
            _combatEventBus = combatEventBus;
            _commandQueue = commandQueue;
            _unit = unit;
        }

        public bool CanInteract => _canInteract.Invoke();
        
        public int TakeDamage(int damage)
        {
            if (!CanInteract)
            {
                throw new();
            }
            

            return _doDamage.Invoke(damage);
        }

        public int DealDamage(IUnit target)
        {
            var targetDamageable = target.GetCombatFeature<IDamageable>();
            if (!CanInteract || !targetDamageable.CanInteract)
            {
                return 0;
            }

            var damage = _getDamage.Invoke();
            if (_combatEventBus.Raise(new PreDamageEvent(Unit, targetDamageable.Unit, damage)))
            {
                return 0;
            }
            
            var result = targetDamageable.TakeDamage(damage);
            // по-хорошему перенести выше и ввести CalcTakeDamage damage?
            _commandQueue.Add(new AttackCommand(Unit, targetDamageable.Unit, result, _commandQueue.Time)); 
            if (!targetDamageable.CanInteract) // а это убрать внутрь TakeDamage
            {
                _commandQueue.Add(new DeathCommand(targetDamageable.Unit, _commandQueue.Time));
            }
            
            if (_combatEventBus.Raise(new AfterDamageEvent(Unit, targetDamageable.Unit, result)))
            {
                return result;
            }
            return result;
        }
    }
}