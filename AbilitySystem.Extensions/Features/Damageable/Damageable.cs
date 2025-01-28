using System;

namespace AbilitySystem
{
    public sealed class Damageable : IDamageable
    {
        private readonly Func<IUnit, bool> _canInteract;
        private readonly Func<IUnit, int, int> _doDamage;
        private readonly Func<IUnit, int> _getDamage;
        
        private ICombatEventBus? _combatEventBus;
        private ICommandQueue? _commandQueue;
        private IUnit? _unit;

        public Damageable(IUnitId unitId,
            Func<IUnit, bool> canInteract,
            Func<IUnit, int, int> doDamage,
            Func<IUnit, int> getDamage)
        {
            UnitId = unitId;
            
            _canInteract = canInteract;
            _doDamage = doDamage;
            _getDamage = getDamage;
        }

        public void Subscribe(ICommandQueue commandQueue, ICombatEventBus combatEventBus)
        {
            _combatEventBus = combatEventBus;
            _commandQueue = commandQueue;
            _unit = _combatEventBus.GetUnit(UnitId);
        }

        public void UnSubscribe()
        {
            _combatEventBus = null;
            _commandQueue = null;
            _unit = null;
        }

        public IUnitId UnitId { get; }
        
        public bool CanInteract
        {
            get
            {
                if (_unit == null)
                {
                    throw new();
                }
                return _canInteract.Invoke(_unit);
            }
        }

        public int TakeDamage(int damage)
        {
            if (!CanInteract || _unit == null)
            {
                throw new();
            }
            
            return _doDamage.Invoke(_unit, damage);
        }

        public int DealDamage(IUnit target)
        {
            if (_combatEventBus == null || _commandQueue == null || _unit == null)
            {
                throw new();
            }
            
            var targetDamageable = target.GetCombatFeature<IDamageable>();
            if (!CanInteract || !targetDamageable.CanInteract)
            {
                return 0;
            }

            var damage = _getDamage.Invoke(_unit);
            
            if (_combatEventBus.Raise(new PreDamageEvent(_unit, target, damage)))
            {
                return 0;
            }
            
            var result = targetDamageable.TakeDamage(damage);
            
            // по-хорошему перенести выше и ввести CalcTakeDamage damage?
            _commandQueue.Add(new AttackCommand(_unit.Id, targetDamageable.UnitId, result, _commandQueue.Time)); 
            if (!targetDamageable.CanInteract) // а это убрать внутрь TakeDamage
            {
                _commandQueue.Add(new DeathCommand(targetDamageable.UnitId, _commandQueue.Time));
            }
            
            if (_combatEventBus.Raise(new AfterDamageEvent(_unit, target, result)))
            {
                return result;
            }
            return result;
        }

        public ICombatFeature DeepClone()
        {
            return new Damageable(UnitId, _canInteract, _doDamage, _getDamage);
        }
    }
}