using System;

namespace AbilitySystem
{
    public sealed class DefenderAbility : IAbility
    {
        public IUnit Owner { get; }

        public DefenderAbility(IUnit owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public IUnit? Redirect(AttackCommand attack)
        {
            return Owner.IsDead
                || ReferenceEquals(Owner, attack.Attacker)
                || ReferenceEquals(Owner, attack.Target)
                ? null
                : Owner;
        }
    }
}
