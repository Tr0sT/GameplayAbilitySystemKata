using System;
using System.Collections.Generic;

namespace AbilitySystem
{
    public sealed class BullyAbility : IAbility
    {
        public IUnit Owner { get; }

        public BullyAbility(IUnit owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public IEnumerable<AttackCommand> React(AttackCommand resolvedAttack)
        {
            if (Owner.IsDead || Owner.Damage <= 0 || resolvedAttack.Damage <= 0
                || resolvedAttack.Target.IsDead
                || ReferenceEquals(Owner, resolvedAttack.Attacker)
                || ReferenceEquals(Owner, resolvedAttack.Target))
                yield break;

            yield return new AttackCommand(Owner, resolvedAttack.Target, Owner.Damage);
        }
    }
}
