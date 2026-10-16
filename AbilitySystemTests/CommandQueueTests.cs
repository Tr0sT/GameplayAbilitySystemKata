using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem;
using NUnit.Framework;

namespace AbilitySystemTests
{
    public sealed class CommandQueueTests
    {
        private Unit _a = null!;
        private Unit _b = null!;
        private Unit _c = null!;
        private Unit _d = null!;
        private Unit _e = null!;

        [SetUp]
        public void SetUp()
        {
            _a = new Unit("A", 10, 1);
            _b = new Unit("B", 10, 1);
            _c = new Unit("C", 10, 1);
            _d = new Unit("D", 10, 1);
            _e = new Unit("E", 10, 1);
        }

        [Test]
        public void AddIsDeferredAndRootAttacksAreFifo()
        {
            var queue = new CommandQueue();
            var first = new AttackCommand(_a, _b, 2);
            var second = new AttackCommand(_c, _d, 3);
            queue.Add(first);
            queue.Add(second);
            Assert.That(_b.Health, Is.EqualTo(10));
            Assert.That(_d.Health, Is.EqualTo(10));

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[] { first, second }));
            Assert.That(_b.Health, Is.EqualTo(8));
            Assert.That(_d.Health, Is.EqualTo(7));
        }

        [Test]
        public void ResultsAreReadOnlySnapshotsAndRepeatedReadsDoNotReplayDamage()
        {
            var queue = new CommandQueue();
            var attack = new AttackCommand(_a, _b, 1);
            Assert.That(queue.GetResult(), Is.Empty);
            queue.Add(attack);
            var first = queue.GetResult();
            Assert.That(queue.GetResult(), Is.EqualTo(first));
            Assert.That(_b.Health, Is.EqualTo(9));
            Assert.Throws<NotSupportedException>(() => ((IList<ICommand>)first).Add(attack));

            queue.Add(attack);
            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[] { attack, attack }));
            Assert.That(first, Is.EqualTo(new ICommand[] { attack }));
            Assert.That(_b.Health, Is.EqualTo(8));
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DeadParticipantsAreCheckedWhenExecuting(bool deadAttacker, bool deadTarget)
        {
            var queue = new CommandQueue(new DefenderAbility(_e));
            queue.Add(new AttackCommand(_a, _b, 1));
            if (deadAttacker) _a.TakeDamage(10);
            if (deadTarget) _b.TakeDamage(10);

            Assert.That(queue.GetResult(), Is.Empty);
            Assert.That(_e.Health, Is.EqualTo(10));
        }

        [Test]
        public void AUnitKilledEarlierCannotExecuteItsQueuedAttackOrBullyReaction()
        {
            var queue = new CommandQueue(new BullyAbility(_c));
            queue.Add(new AttackCommand(_a, _c, 10));
            queue.Add(new AttackCommand(_c, _b, 1));
            queue.Add(new AttackCommand(_a, _b, 1));

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[]
            {
                new AttackCommand(_a, _c, 10), new DeathCommand(_c),
                new AttackCommand(_a, _b, 1)
            }));
            Assert.That(_b.Health, Is.EqualTo(9));
        }

        [Test]
        public void OverkillRecordsActualDamageAndExactlyOneDeath()
        {
            var queue = new CommandQueue();
            queue.Add(new AttackCommand(_a, _b, int.MaxValue));
            queue.Add(new AttackCommand(_c, _b, 1));

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[]
            {
                new AttackCommand(_a, _b, 10), new DeathCommand(_b)
            }));
            Assert.That(_b.Health, Is.Zero);
        }

        [TestCase("attacker")]
        [TestCase("target")]
        [TestCase("dead")]
        [TestCase("zero-outgoing")]
        [TestCase("zero-incoming")]
        public void IneligibleBulliesDoNotReact(string reason)
        {
            var owner = reason switch
            {
                "attacker" => _a,
                "target" => _b,
                "zero-outgoing" => new Unit("C", 10, 0),
                _ => _c
            };
            if (reason == "dead") owner.TakeDamage(10);
            var attack = new AttackCommand(_a, _b, reason == "zero-incoming" ? 0 : 1);
            var queue = new CommandQueue(new BullyAbility(owner));
            queue.Add(attack);

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[] { attack }));
        }

        [Test]
        public void UnitIdentityIsNotItsDisplayName()
        {
            var sameName = new Unit("A", 10, 1);
            var queue = new CommandQueue(new BullyAbility(sameName));
            queue.Add(new AttackCommand(_a, _b, 1));

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[]
            {
                new AttackCommand(_a, _b, 1), new AttackCommand(sameName, _b, 1)
            }));
        }

        [Test]
        public void BattlesDoNotShareAbilitiesAndRegistrationArraysAreCopied()
        {
            var abilities = new IAbility[] { new BullyAbility(_c) };
            var first = new CommandQueue(abilities);
            abilities[0] = new BullyAbility(_d);
            var second = new CommandQueue(abilities);
            first.Add(new AttackCommand(_a, _b, 1));
            second.Add(new AttackCommand(_a, _e, 1));

            Assert.That(first.GetResult(), Is.EqualTo(new ICommand[]
            {
                new AttackCommand(_a, _b, 1), new AttackCommand(_c, _b, 1)
            }));
            Assert.That(second.GetResult(), Is.EqualTo(new ICommand[]
            {
                new AttackCommand(_a, _e, 1), new AttackCommand(_d, _e, 1)
            }));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ThreeBulliesUseRegistrationOrderAndDepthFirstChains(bool reverse)
        {
            _b.TakeDamage(5);
            var first = reverse ? _d : _c;
            var second = reverse ? _c : _d;
            var queue = new CommandQueue(
                new BullyAbility(first), new BullyAbility(second), new BullyAbility(_e));
            queue.Add(new AttackCommand(_a, _b, 1));

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[]
            {
                new AttackCommand(_a, _b, 1), new AttackCommand(first, _b, 1),
                new AttackCommand(second, _b, 1), new AttackCommand(first, _b, 1),
                new AttackCommand(second, _b, 1), new DeathCommand(_b)
            }));
        }

        [Test]
        public void LongReactionChainsDoNotUseTheCallStack()
        {
            const int health = 20_000;
            var target = new Unit("Target", health, 0);
            var queue = new CommandQueue(new BullyAbility(_c), new BullyAbility(_d));
            queue.Add(new AttackCommand(_a, target, 1));
            var result = queue.GetResult();

            Assert.That(result.Count, Is.EqualTo(health + 1));
            Assert.That(result[0], Is.EqualTo(new AttackCommand(_a, target, 1)));
            for (var i = 1; i < health; ++i)
                Assert.That(result[i], Is.EqualTo(new AttackCommand(i % 2 == 1 ? _c : _d, target, 1)));
            Assert.That(result[health], Is.EqualTo(new DeathCommand(target)));
            Assert.That(target.Health, Is.Zero);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FirstEligibleDefenderWinsWithoutRedirectionChains(bool reverse)
        {
            var first = reverse ? _c : _e;
            var second = reverse ? _e : _c;
            var queue = new CommandQueue(new DefenderAbility(first), new DefenderAbility(second));
            queue.Add(new AttackCommand(_a, _b, 2));

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[]
            {
                new TryAttackCommand(_a, _b), new DefendCommand(first, _b),
                new AttackCommand(_a, first, 2)
            }));
            Assert.That(first.Health, Is.EqualTo(8));
            Assert.That(second.Health, Is.EqualTo(10));
            Assert.That(_b.Health, Is.EqualTo(10));
        }

        [TestCase("dead")]
        [TestCase("attacker")]
        [TestCase("target")]
        public void IneligibleDefenderDoesNotIntercept(string reason)
        {
            var owner = reason == "attacker" ? _a : reason == "target" ? _b : _e;
            if (reason == "dead") owner.TakeDamage(10);
            var queue = new CommandQueue(new DefenderAbility(owner));
            var attack = new AttackCommand(_a, _b, 1);
            queue.Add(attack);
            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[] { attack }));
        }

        [Test]
        public void DeadDefendersFallBackToTheNextDefenderThenTheOriginalTarget()
        {
            _e.TakeDamage(9);
            _d.TakeDamage(9);
            var queue = new CommandQueue(new DefenderAbility(_e), new DefenderAbility(_d));
            for (var i = 0; i < 3; ++i) queue.Add(new AttackCommand(_a, _b, 1));

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[]
            {
                new TryAttackCommand(_a, _b), new DefendCommand(_e, _b),
                new AttackCommand(_a, _e, 1), new DeathCommand(_e),
                new TryAttackCommand(_a, _b), new DefendCommand(_d, _b),
                new AttackCommand(_a, _d, 1), new DeathCommand(_d),
                new AttackCommand(_a, _b, 1)
            }));
            Assert.That(_b.Health, Is.EqualTo(9));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void BullyAndDefenderComposeRegardlessOfTheirRelativeRegistrationOrder(bool reverse)
        {
            var abilities = new IAbility[] { new BullyAbility(_c), new DefenderAbility(_e) };
            if (reverse) Array.Reverse(abilities);
            var queue = new CommandQueue(abilities);
            queue.Add(new AttackCommand(_a, _b, 1));

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[]
            {
                new TryAttackCommand(_a, _b), new DefendCommand(_e, _b),
                new AttackCommand(_a, _e, 1), new AttackCommand(_c, _e, 1)
            }));
            Assert.That(_b.Health, Is.EqualTo(10));
            Assert.That(_e.Health, Is.EqualTo(8));
        }

        [Test]
        public void KillingADefenderDoesNotSpillOverOrMakeBulliesHitTheOriginalTarget()
        {
            _e.TakeDamage(8);
            var queue = new CommandQueue(new BullyAbility(_c), new DefenderAbility(_e));
            queue.Add(new AttackCommand(_a, _b, 100));

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[]
            {
                new TryAttackCommand(_a, _b), new DefendCommand(_e, _b),
                new AttackCommand(_a, _e, 2), new DeathCommand(_e)
            }));
            Assert.That(_b.Health, Is.EqualTo(10));
        }

        [Test]
        public void NewAbilityCanReactToAKillUsingFinalTargetAndActualDamage()
        {
            _e.TakeDamage(8);
            AttackCommand? observed = null;
            var onKill = new ReactionAbility(attack =>
            {
                if (!ReferenceEquals(attack.Attacker, _a)) return Array.Empty<AttackCommand>();
                observed = attack;
                return new[] { new AttackCommand(_c, _b, 1) };
            });
            var queue = new CommandQueue(onKill, new DefenderAbility(_e));
            queue.Add(new AttackCommand(_a, _b, 100));

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[]
            {
                new TryAttackCommand(_a, _b), new DefendCommand(_e, _b),
                new AttackCommand(_a, _e, 2), new DeathCommand(_e),
                new AttackCommand(_c, _b, 1)
            }));
            Assert.That(observed, Is.EqualTo(new AttackCommand(_a, _e, 2)));
        }

        [Test]
        public void CustomReactionsKeepChildSiblingAndRootOrder()
        {
            var f = new Unit("F", 10, 1);
            var g = new Unit("G", 10, 1);
            var ability = new ReactionAbility(attack =>
            {
                if (ReferenceEquals(attack.Attacker, _a))
                    return new[] { new AttackCommand(_c, _b, 1), new AttackCommand(_d, _b, 1) };
                if (ReferenceEquals(attack.Attacker, _c))
                    return new[] { new AttackCommand(_e, _b, 1) };
                if (ReferenceEquals(attack.Attacker, _d))
                    return new[] { new AttackCommand(f, _b, 1) };
                return Array.Empty<AttackCommand>();
            });
            var queue = new CommandQueue(ability);
            queue.Add(new AttackCommand(_a, _b, 1));
            queue.Add(new AttackCommand(g, _b, 1));

            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[]
            {
                new AttackCommand(_a, _b, 1), new AttackCommand(_c, _b, 1),
                new AttackCommand(_e, _b, 1), new AttackCommand(_d, _b, 1),
                new AttackCommand(f, _b, 1), new AttackCommand(g, _b, 1)
            }));
        }

        [Test]
        public void ExactCommandBudgetSucceedsAndResetsForTheNextResolution()
        {
            var queue = new CommandQueue(2, new BullyAbility(_c));
            queue.Add(new AttackCommand(_a, _b, 1));
            Assert.That(queue.GetResult().Count, Is.EqualTo(2));
            queue.Add(new AttackCommand(_a, _b, 1));
            Assert.That(queue.GetResult().Count, Is.EqualTo(4));
        }

        [Test]
        public void AnOversizedRootBatchFailsBeforeMutatingUnits()
        {
            var queue = new CommandQueue(1);
            queue.Add(new AttackCommand(_a, _b, 1));
            queue.Add(new AttackCommand(_c, _b, 1));
            Assert.Throws<InvalidOperationException>(() => queue.GetResult());
            Assert.That(_b.Health, Is.EqualTo(10));
        }

        [Test]
        public void NonTerminatingReactionChainsFailWithADiagnosticAndCannotResume()
        {
            var calls = 0;
            var loop = new ReactionAbility(attack =>
            {
                ++calls;
                return new[] { new AttackCommand(_a, _b, 0) };
            });
            var queue = new CommandQueue(5, loop);
            queue.Add(new AttackCommand(_a, _b, 0));

            var error = Assert.Throws<InvalidOperationException>(() => queue.GetResult());
            Assert.That(error!.Message, Does.Contain("command budget"));
            Assert.That(calls, Is.EqualTo(5));
            Assert.That(_b.Health, Is.EqualTo(10));
            Assert.Throws<InvalidOperationException>(() => queue.GetResult());
            Assert.Throws<InvalidOperationException>(() => queue.Add(new AttackCommand(_a, _b, 1)));
        }

        [Test]
        public void InfiniteReactionEnumerationsAreBoundedAndDisposed()
        {
            var enumerated = 0;
            var disposed = false;
            IEnumerable<AttackCommand> Endless(AttackCommand attack)
            {
                try
                {
                    while (true)
                    {
                        ++enumerated;
                        yield return new AttackCommand(_c, _b, 0);
                    }
                }
                finally { disposed = true; }
            }
            var queue = new CommandQueue(5, new ReactionAbility(Endless));
            queue.Add(new AttackCommand(_a, _b, 1));

            Assert.Throws<InvalidOperationException>(() => queue.GetResult());
            Assert.That(enumerated, Is.EqualTo(5));
            Assert.That(disposed, Is.True);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ReentrantReadsAndWritesAreRejected(bool write)
        {
            CommandQueue queue = null!;
            var ability = new ReactionAbility(attack =>
            {
                if (write) queue.Add(new AttackCommand(_c, _b, 1));
                else queue.GetResult();
                return Array.Empty<AttackCommand>();
            });
            queue = new CommandQueue(ability);
            queue.Add(new AttackCommand(_a, _b, 1));
            var error = Assert.Throws<InvalidOperationException>(() => queue.GetResult());
            Assert.That(error!.Message, Does.Contain("re-enter"));
            Assert.Throws<InvalidOperationException>(() => queue.GetResult());
        }

        [TestCase("null")]
        [TestCase("attacker")]
        [TestCase("target")]
        [TestCase("damage")]
        public void InvalidReactionsAreRejectedAndFaultTheBattle(string reason)
        {
            var invalid = reason switch
            {
                "null" => null!,
                "attacker" => new AttackCommand(null!, _b, 1),
                "target" => new AttackCommand(_c, null!, 1),
                _ => new AttackCommand(_c, _b, -1)
            };
            var queue = new CommandQueue(new ReactionAbility(_ => new[] { invalid }));
            queue.Add(new AttackCommand(_a, _b, 1));
            Assert.Catch<ArgumentException>(() => queue.GetResult());
            Assert.That(_b.Health, Is.EqualTo(9)); // Failure is not a rollback.
            Assert.Throws<InvalidOperationException>(() => queue.GetResult());
        }

        [Test]
        public void InvalidInputsAreRejectedWithoutPoisoningTheQueue()
        {
            var queue = new CommandQueue();
            Assert.Throws<ArgumentNullException>(() => queue.Add(null!));
            Assert.Throws<ArgumentException>(() => queue.Add(new DeathCommand(_b)));
            Assert.Throws<ArgumentException>(() => queue.Add(new TryAttackCommand(_a, _b)));
            Assert.Throws<ArgumentException>(() => queue.Add(new DefendCommand(_e, _b)));
            Assert.Throws<ArgumentException>(() => queue.Add(new AttackCommand(null!, _b, 1)));
            Assert.Throws<ArgumentException>(() => queue.Add(new AttackCommand(_a, null!, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => queue.Add(new AttackCommand(_a, _b, -1)));
            var attack = new AttackCommand(_a, _b, 1);
            queue.Add(attack);
            Assert.That(queue.GetResult(), Is.EqualTo(new ICommand[] { attack }));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void NonPositiveBudgetsAreRejected(int budget)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CommandQueue(budget));
        }

        [Test]
        public void NullAbilitiesAndOwnersAreRejected()
        {
            Assert.Throws<ArgumentNullException>(() => new CommandQueue((IAbility[])null!));
            Assert.Throws<ArgumentException>(() => new CommandQueue(new IAbility[] { null! }));
            Assert.Throws<ArgumentNullException>(() => new BullyAbility(null!));
            Assert.Throws<ArgumentNullException>(() => new DefenderAbility(null!));
        }

        private sealed class ReactionAbility : IAbility
        {
            private readonly Func<AttackCommand, IEnumerable<AttackCommand>> _react;

            public ReactionAbility(Func<AttackCommand, IEnumerable<AttackCommand>> react)
            {
                _react = react;
            }

            public IEnumerable<AttackCommand> React(AttackCommand resolvedAttack) => _react(resolvedAttack);
        }
    }
}
