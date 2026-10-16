using System;
using AbilitySystem;
using NUnit.Framework;

namespace AbilitySystemTests
{
    public sealed class UnitTests
    {
        [TestCase(0, 5)]
        [TestCase(3, 2)]
        [TestCase(5, 0)]
        [TestCase(9, 0)]
        [TestCase(int.MaxValue, 0)]
        public void DamageIsClampedToRemainingHealth(int damage, int expectedHealth)
        {
            var unit = new Unit("A", 5, 1);
            unit.TakeDamage(damage);
            Assert.That(unit.Health, Is.EqualTo(expectedHealth));
            Assert.That(unit.IsDead, Is.EqualTo(expectedHealth == 0));
        }

        [Test]
        public void DeadUnitsStayDeadAndCannotDealDirectDamage()
        {
            var dead = new Unit("Dead", 0, 10);
            var target = new Unit("Target", 5, 0);
            dead.TakeDamage(int.MaxValue);
            dead.DealDamage(target);
            Assert.That(dead.Health, Is.Zero);
            Assert.That(dead.IsDead, Is.True);
            Assert.That(target.Health, Is.EqualTo(5));
        }

        [Test]
        public void DirectDamageUsesTheSameHealthRules()
        {
            var attacker = new Unit("A", 5, 10);
            var target = new Unit("B", 3, 0);
            attacker.DealDamage(target);
            Assert.That(target.Health, Is.Zero);
            Assert.That(target.IsDead, Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void BlankNamesAreRejected(string? name)
        {
            Assert.Throws<ArgumentException>(() => new Unit(name!, 1, 1));
        }

        [Test]
        public void NegativeStatsDamageAndNullTargetsAreRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Unit("A", -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Unit("A", 1, -1));
            var unit = new Unit("A", 5, 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => unit.TakeDamage(-1));
            Assert.Throws<ArgumentNullException>(() => unit.DealDamage(null!));
            Assert.That(unit.Health, Is.EqualTo(5));
        }
    }
}
