using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public sealed class Unit : IUnit
    {
        private readonly CommandQueue _commandQueue;
        private readonly List<Func<IUnit, IUnit, int, bool>> _preDamageSubscribers = new();
        private readonly List<Func<IUnit, IUnit, int, bool>> _afterDamageSubscribers = new();
        
        private readonly List<IStatusEffect> _statusEffects = new();
        public string Name { get; }
        public int Health { get; private set; }
        public int Damage { get; private set; }
        public bool IsDead => Health <= 0;
        
        public Unit(string name, int health, int damage, CommandQueue commandQueue)
        {
            _commandQueue = commandQueue!;
            Name = name;
            Health = health;
            Damage = damage;
        }

        public int TakeDamage(int damage)
        {
            if (IsDead)
            {
                throw new System.Exception("Unit is already dead");
            }

            var realDamage = Math.Min(Health, damage);
            Health -= realDamage;

            return realDamage;
        }

        public int DealDamage(IUnit target)
        {
            if (IsDead || target.IsDead)
            {
                return 0;
            }
            
            foreach (var preDamageSubscriber in _preDamageSubscribers)
            {
                var cancel = preDamageSubscriber.Invoke(this, target, Damage);
                if (cancel)
                {
                    return 0;
                }
            }
            
            var result = target.TakeDamage(Damage);
            _commandQueue.Add(new AttackCommand(this, target, result)); // по-хорошему перенести выше и ввести CalcTakeDamage damage?
            if (target.IsDead) // а это убрать внутрь TakeDamage
            {
                _commandQueue.Add(new DeathCommand(target));
            }
            
            foreach (var subscriber in _afterDamageSubscribers)
            {
                var cancel = subscriber.Invoke(this, target, result);
                if (cancel)
                {
                    return result;
                }
            }
            return result;
        }

        public ReadOnlyCollection<IStatusEffect> StatusEffects => _statusEffects.AsReadOnly();
        public void AddStatusEffect(IStatusEffect statusEffect, List<IUnit> units)
        {
            _statusEffects.Add(statusEffect);
            statusEffect.Init(this, units);
        }

        public void SubscribeToPreDamage(Func<IUnit, IUnit, int, bool> func)
        {
            _preDamageSubscribers.Add(func);
        }

        public void SubscribeToAfterDamage(Func<IUnit, IUnit, int, bool> func)
        {
            _afterDamageSubscribers.Add(func);
        }
    }
}
