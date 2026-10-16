# Architectural Kata: Command Queue and Abilities System for a Turn-Based Game

## Task
Implement a system that forms an event queue for a turn-based game. The queue should contain information about all interactions between units during combat.

## Description of Abilities
- **Bully:**  
  A unit that reacts to an attack by adding its own attack to the queue against the same target, provided the target is still alive.
  
- **Defender:**  
  A unit that redirects all attacks aimed at another unit to itself.

## Scenarios
1. **Double strike with one bully:**  
   - Unit A performs two strikes against B.  
   - Bully C inserts its attack after each strike by A.

2. **Double strike with a kill:**  
   - Unit A performs two strikes against B.  
   - B dies from the first strike, and subsequent commands targeting B are ignored.

3. **Two bullies:**  
   - A attacks B.  
   - Bullies C and D react to the attack, attacking B until it dies.

4. **Defender:**  
   - A attacks B.  
   - Defender E redirects A's attack to itself.

## Requirements
- The command queue is formed and processed correctly.
- Unit abilities (bullies, defender) operate independently.
- The code is easily extendable for adding new abilities.

## Solution

The resolver separates pending **attack intentions** from the **journal of resolved events**.
Abilities are ordinary, explicitly registered objects with two independent hooks:
`Redirect` before damage and `React` after damage/death. There is no global unit registry,
event bus, or ability-specific branching in `CommandQueue` or `Unit`.

```csharp
var a = new Unit("A", 5, 1);
var b = new Unit("B", 5, 0);
var c = new Unit("C", 5, 1);
var queue = new CommandQueue(new BullyAbility(c));

queue.Add(new AttackCommand(a, b, a.Damage));
queue.Add(new AttackCommand(a, b, a.Damage));
var events = queue.GetResult(); // A -> B, C -> B, A -> B, C -> B
// b.Health == 1
```

Reactions resolve **depth-first**, in registration order, before the next queued strike.
A linked-list work queue makes this iterative even for long reaction chains. Dead
participants are skipped at execution time; damage in the journal is actual damage,
not overkill. Each result is a read-only snapshot, and repeated reads do not replay combat.

The four original scenario assertions are retained; only their setup now registers
abilities explicitly instead of using the unimplemented `Unit.ReactOnDamage...` methods.
See [the design notes](docs/solution.md) for ordering, ambiguous cases, extension examples,
and intentional limitations.

## Run the tests

The solution retains the original .NET 6 / C# 9 target and NUnit dependencies in the
**test project only**. Install the .NET 6 SDK (latest patch); `global.json` selects its
latest installed feature band rather than a different major SDK.

```sh
dotnet test AbilitySystemSolution.sln --configuration Debug
dotnet test AbilitySystemSolution.sln --configuration Release
```

GitHub Actions runs both configurations on pull requests and pushes to `main`.
The tests cover the original scenarios, mixed abilities, ordering, deaths, snapshots,
20,000-strike chains, new ability implementations, and invalid/cyclic extensions.

.NET 6 is retained for kata compatibility, not recommended for new production deployments;
upgrading the target and test tooling is deliberately a separate change.
