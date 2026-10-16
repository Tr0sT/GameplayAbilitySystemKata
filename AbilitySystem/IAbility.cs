using System;
using System.Collections.Generic;

namespace AbilitySystem
{
    /// <summary>
    /// Battle-local attack hooks. Implement only the phase an ability needs.
    /// Hooks must not mutate units or call back into the queue: return intentions instead.
    /// </summary>
    public interface IAbility
    {
        /// <summary>
        /// Propose a replacement target before damage. The first living replacement
        /// other than the attacker or original target wins; redirection is not recursive.
        /// </summary>
        IUnit? Redirect(AttackCommand attack) => null;

        /// <summary>
        /// React after damage and any death have been recorded. The attack contains
        /// the final target and actual damage. Returned attacks run in enumeration order,
        /// with each one's entire reaction chain resolved before its next sibling.
        /// </summary>
        IEnumerable<AttackCommand> React(AttackCommand resolvedAttack)
            => Array.Empty<AttackCommand>();
    }
}
