using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    /// <summary>
    /// A single-threaded battle resolver. Add queues intentions; GetResult resolves
    /// them and returns a snapshot of the cumulative, ordered combat journal.
    /// </summary>
    public sealed class CommandQueue : ICommandQueue
    {
        private const int DefaultCommandBudget = 100_000;
        private readonly LinkedList<AttackCommand> _pending = new();
        private readonly List<ICommand> _history = new();
        private readonly IAbility[] _abilities;
        private readonly int _maxCommandsPerResolution;
        private bool _isResolving;
        private bool _isFaulted;

        public CommandQueue(params IAbility[] abilities)
            : this(DefaultCommandBudget, abilities) { }

        public CommandQueue(int maxCommandsPerResolution, params IAbility[] abilities)
        {
            if (maxCommandsPerResolution <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxCommandsPerResolution));
            if (abilities is null)
                throw new ArgumentNullException(nameof(abilities));

            _abilities = (IAbility[])abilities.Clone();
            foreach (var ability in _abilities)
                if (ability is null)
                    throw new ArgumentException("Abilities cannot contain null.", nameof(abilities));

            _maxCommandsPerResolution = maxCommandsPerResolution;
        }

        public void Add(ICommand command)
        {
            EnsureIdle();
            if (command is null)
                throw new ArgumentNullException(nameof(command));
            if (command is not AttackCommand attack)
                throw new ArgumentException("Only attack intentions can be queued; other commands are results.", nameof(command));

            ValidateAttack(attack);
            _pending.AddLast(attack);
        }

        public ReadOnlyCollection<ICommand> GetResult()
        {
            EnsureIdle();
            _isResolving = true;
            try
            {
                if (_pending.Count > _maxCommandsPerResolution)
                    throw BudgetExceeded();

                var processed = 0;
                while (_pending.First is not null)
                {
                    var attack = _pending.First.Value;
                    _pending.RemoveFirst();
                    Resolve(attack, ++processed);
                }

                // Do not expose a live view: a later resolution must not change this result.
                return new List<ICommand>(_history).AsReadOnly();
            }
            catch
            {
                // Resolution mutates units; it is deliberately not a transaction.
                // Never allow a failed, partially resolved battle to resume silently.
                _isFaulted = true;
                throw;
            }
            finally
            {
                _isResolving = false;
            }
        }

        private void Resolve(AttackCommand intention, int processed)
        {
            // Validate liveness at execution time, not when an intention is added.
            if (intention.Attacker.IsDead || intention.Target.IsDead)
                return;

            var target = intention.Target;
            foreach (var ability in _abilities)
            {
                var replacement = ability.Redirect(intention);
                if (replacement is null || replacement.IsDead
                    || ReferenceEquals(replacement, intention.Attacker)
                    || ReferenceEquals(replacement, intention.Target))
                    continue;

                target = replacement;
                _history.Add(new TryAttackCommand(intention.Attacker, intention.Target));
                _history.Add(new DefendCommand(target, intention.Target));
                break;
            }

            var healthBefore = target.Health;
            target.TakeDamage(intention.Damage);
            var resolved = new AttackCommand(intention.Attacker, target, healthBefore - target.Health);
            _history.Add(resolved);
            if (target.IsDead)
                _history.Add(new DeathCommand(target));

            // Invoke even on lethal/zero-damage attacks: an on-kill ability can target
            // somebody else. Bully owns its own eligibility rules, not the resolver.
            var reactions = new List<AttackCommand>();
            foreach (var ability in _abilities)
            {
                foreach (var reaction in ability.React(resolved))
                {
                    ValidateAttack(reaction);
                    // Bound both execution and enumeration (including an infinite iterator).
                    if ((long)processed + _pending.Count + reactions.Count >= _maxCommandsPerResolution)
                        throw BudgetExceeded();
                    reactions.Add(reaction);
                }
            }

            // Prepend in reverse to preserve registration/enumeration order. Children
            // precede siblings and previously queued root attacks, without recursion.
            for (var i = reactions.Count - 1; i >= 0; --i)
                _pending.AddFirst(reactions[i]);
        }

        private static void ValidateAttack(AttackCommand attack)
        {
            if (attack is null)
                throw new ArgumentNullException(nameof(attack));
            if (attack.Attacker is null || attack.Target is null)
                throw new ArgumentException("An attack requires an attacker and a target.", nameof(attack));
            if (attack.Damage < 0)
                throw new ArgumentOutOfRangeException(nameof(attack), "Damage cannot be negative.");
        }

        private void EnsureIdle()
        {
            if (_isFaulted)
                throw new InvalidOperationException("This battle failed during resolution and cannot be reused.");
            if (_isResolving)
                throw new InvalidOperationException("Abilities must return reactions, not re-enter the command queue.");
        }

        private InvalidOperationException BudgetExceeded()
        {
            return new InvalidOperationException(
                $"The resolution exceeded its {_maxCommandsPerResolution} command budget. Check for a reaction cycle.");
        }
    }
}
