# Design: resolve intentions, record facts

## Keep the kata small

`Unit` owns health and basic damage invariants. `CommandQueue` owns scheduling and the
combat journal. `IAbility` exposes two phases, with no-op defaults so an ability only
implements the phase it needs. `BullyAbility` and `DefenderAbility` do not know each other.
The runtime library has no package dependencies.

Abilities are passed into a queue in a fixed order. This is the battle boundary:
a unit existing somewhere in the process must not silently participate in another battle.
The queue copies the registration array. Units use reference identity, not display names.
The old unimplemented `Unit.ReactOnDamage...` helpers are replaced by explicit composition.

There is no service locator, static subscription, reflection, priority framework, or
ability-type switch in the resolver. Those mechanisms would obscure the scheduling rule
without helping these scenarios.

## One attack, two phases

1. Skip an intention if its attacker or its original target is already dead.
2. Ask abilities for a replacement target, in registration order. The first living
   replacement other than the attacker/original target wins. Stop after that redirect.
3. If redirected, record `TryAttackCommand(original)` and `DefendCommand`.
4. Apply damage and record `AttackCommand(final target, actual damage)`.
5. Record `DeathCommand` immediately if this attack killed the final target.
6. Ask abilities for reactions to the resolved attack, including lethal/zero-damage
   attacks. Put those intentions before existing work, retaining their declared order.

The existing `ICommand` record names and `ICommandQueue` signature remain compatible with
the acceptance assertions. They describe two roles: only `AttackCommand` is accepted as
input; `TryAttackCommand`, `DefendCommand`, and `DeathCommand` are output facts, never
executable work. A queued attack's damage is requested damage; its journal entry records
the health delta. There is no attempt/defend noise for an ordinary, unredirected attack.

## Scheduling is a gameplay rule, not an implementation accident

Root intentions are FIFO. Reactions are **depth-first**: finish a child's chain before
its next sibling, then continue the next root. A linked list gives `AddLast` for root
intentions and `AddFirst` for reactions; the latter are inserted in reverse order.
Resolution is a loop, not recursive event dispatch.

With two bullies, A's first attack creates C and D. C's attack creates a new D; that
child runs before the original D sibling and creates C, and so on:

```text
A -> C -> D -> C -> D -> death
```

Any pending sibling or second root aimed at the dead target is then skipped. Appending
all reactions to a FIFO would instead produce `A,C,D,D,C,...`, which contradicts the
kata's exact expected sequence. Suppressing all reactions to reactions would fail too.

With three bullies this policy is intentionally *not round-robin*: early siblings can
kill the target before a later bully acts. The regression tests make this explicit.
Registration order is the tie-breaker, not a claim about fairness.

## Policies for underspecified cases

- Bully reacts to positive damage to another living unit. It never reacts to its own
  attack, attacks on itself, or when dead/zero-damage. Two zero-damage bullies cannot
  create a non-progressing chain. Other abilities may still react to zero-damage attacks.
- Defender protects other targets, but cannot intercept its own outgoing attack or
  an attack already aimed at itself. With multiple defenders, first eligible wins.
  There is no recursive redirect/ping-pong within a single attack.
- A dead original target is not rescued by redirecting its stale command. A dead
  defender is ignored by later attacks. Overkill is absorbed, never spilled into the
  protected unit. A bully reacts to the final target, not the protected original target.
- Death is recorded once, immediately after the lethal attack. General reaction hooks
  still see the lethal result, allowing on-kill follow-ups against a different target.
- `Add` does not mutate health. `GetResult` drains work and returns a **cumulative**
  read-only snapshot. Repeated reads are idempotent; later additions extend only new
  snapshots. Invalid input is rejected before it enters the work queue.

There are no factions in the kata; therefore no invented ally/enemy rule is hidden
inside either ability.

## An extension without modifying the resolver

For example, an ability can attack a different target after its owner's kill:

```csharp
public sealed class FollowUpOnKill : IAbility
{
    private readonly IUnit _owner;
    private readonly IUnit _nextTarget;

    public FollowUpOnKill(IUnit owner, IUnit nextTarget)
    {
        _owner = owner;
        _nextTarget = nextTarget;
    }

    public IEnumerable<AttackCommand> React(AttackCommand attack)
    {
        if (ReferenceEquals(attack.Attacker, _owner)
            && attack.Target.IsDead && !_owner.IsDead && !_nextTarget.IsDead)
            yield return new AttackCommand(_owner, _nextTarget, _owner.Damage);
    }
}
```

Register it alongside other abilities. The same resolver handles its scheduling,
redirection, actual damage and deaths. Tests also define a new reaction implementation
for nested child/sibling ordering and on-kill behavior, without a production-code change.

## Boundaries and failure semantics

This is a single-threaded, in-memory attack resolver, not a complete GAS or an event-sourced
engine. `IUnit` remains mutable for compatibility, and event records hold unit references:
collection snapshots are stable, but they are not historical snapshots of unit health.
Direct `DealDamage` is retained as a low-level helper and deliberately bypasses the queue.
Use queued attacks for abilities and a complete journal.

Ability hooks are trusted code: they should inspect state and return intentions, not
mutate units, block, or re-enter the queue. Reentrant `Add`/`GetResult` is rejected.
The configurable command budget (default 100,000 per `GetResult`) bounds both total
scheduled work and reaction enumeration, including skipped intentions. It detects
non-terminating reaction chains and infinite yielding iterators; it cannot preempt a
hook that itself never returns. It is a diagnostic guard, not a substitute for gameplay
rules. A legitimate larger battle can choose a larger budget with
`new CommandQueue(maxCommandsPerResolution: 200_000, abilities: abilities)`.

Resolution is not transactional. If a hook throws or exceeds the budget, earlier
health changes are not rolled back. The queue becomes faulted and rejects reuse rather
than pretending a partially resolved battle can be retried safely. For save/load,
network rollback, or untrusted abilities, use stable unit IDs, explicit state snapshots,
and a stronger command/event boundary as a separate design exercise.
