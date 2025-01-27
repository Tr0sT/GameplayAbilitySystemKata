using NUnit.Framework;
using AbilitySystem;

namespace AbilitySystemTests
{
    public sealed class AbilitySystemTests
    {
        [Test]
        public void DoubleStrikeWithOneBully_Full()
        {
            var combatEventsContext = new CombatEventsContext();
            var commandQueue = new CommandQueue(combatEventsContext);
            var combatContext = new CombatContext(commandQueue);
            
            var attackerA = new Unit("A", 5, 1, commandQueue, combatEventsContext);
            var targetB = new Unit("B", 5, 0, commandQueue, combatEventsContext);
            var bullyC = new Unit("C", 5, 1, commandQueue, combatEventsContext);
            
            bullyC.AddBullyStatusEffect();

            DoubleAttackUsingProjectiles(combatContext, attackerA, targetB);
            
            // Anything above can be changed, but the result must be correct:
            var result = commandQueue.CalcResult();
            Assert.AreEqual(6, result.Count); 
            Assert.Contains(new CreateProjectileCommand(attackerA, targetB, 10, 0), result);
            Assert.Contains(new AttackCommand(attackerA, targetB, attackerA.Damage, 10), result);
            Assert.Contains(new AttackCommand(bullyC, targetB, bullyC.Damage, 10), result);
            Assert.Contains(new CreateProjectileCommand(attackerA, targetB, 20, 5), result);
            Assert.Contains(new AttackCommand(attackerA, targetB, attackerA.Damage, 25), result);
            Assert.Contains(new AttackCommand(bullyC, targetB, bullyC.Damage, 25), result);
        }
        

        private void DoubleAttackUsingProjectiles(ICombatContext combatContext, IUnit source, IUnit target)
        {
            combatContext.CreateProjectile(source, target, 10, () =>
            {
                source.GetCombatFeature<IDamageable>().DealDamage(target);
            });
            combatContext.Delay(5, () =>
            {
                combatContext.CreateProjectile(source, target, 20, () =>
                {
                    source.GetCombatFeature<IDamageable>().DealDamage(target);
                });
            });
        }
        
        [Test]
        public void DoubleStrikeWithOneBully_OneShot()
        {
            var combatEventsContext = new CombatEventsContext();
            var commandQueue = new CommandQueue(combatEventsContext);

            var attackerA = new Unit("A", 5, 10, commandQueue, combatEventsContext);
            var targetB = new Unit("B", 5, 0, commandQueue, combatEventsContext);
            var bullyC = new Unit("C", 5, 1, commandQueue, combatEventsContext);
            bullyC.AddBullyStatusEffect();
            
            attackerA.GetCombatFeature<IDamageable>().DealDamage(targetB);
            attackerA.GetCombatFeature<IDamageable>().DealDamage(targetB);
            
            // Anything above can be changed, but the result must be correct:
            var result = commandQueue.CalcResult();
            Assert.AreEqual(2, result.Count); 
            Assert.AreEqual(new AttackCommand(attackerA, targetB, 5, 0), result[0]);
            Assert.AreEqual(new DeathCommand(targetB, 0), result[1]);
        }
        
        [Test]
        public void DoubleStrikeWithTwoBully_Full()
        {
            var combatEventsContext = new CombatEventsContext();
            var commandQueue = new CommandQueue(combatEventsContext);

            var attackerA = new Unit("A", 5, 1, commandQueue, combatEventsContext);
            var targetB = new Unit("B", 5, 0, commandQueue, combatEventsContext);
            var bullyC = new Unit("C", 5, 1, commandQueue, combatEventsContext);
            
            var bullyD = new Unit("D", 5, 1, commandQueue, combatEventsContext);
            bullyC.AddBullyStatusEffect();
            bullyD.AddBullyStatusEffect();
            
            attackerA.GetCombatFeature<IDamageable>().DealDamage(targetB);
            attackerA.GetCombatFeature<IDamageable>().DealDamage(targetB);

            // Anything above can be changed, but the result must be correct:
            var result = commandQueue.CalcResult();
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual(new AttackCommand(attackerA, targetB, attackerA.Damage, 0), result[0]); // 4 хп
            Assert.AreEqual(new AttackCommand(bullyC, targetB, bullyC.Damage, 0), result[1]); // 3 хп
            Assert.AreEqual(new AttackCommand(bullyD, targetB, bullyD.Damage, 0), result[2]); // 2 хп
            Assert.AreEqual(new AttackCommand(bullyC, targetB, bullyC.Damage, 0), result[3]); // 1 хп
            Assert.AreEqual(new AttackCommand(bullyD, targetB, bullyD.Damage, 0), result[4]); // 0 хп
            Assert.AreEqual(new DeathCommand(targetB, 0), result[5]);
        }
        
        [Test]
        public void SingleStrikeWithOneDefender_Full()
        {
            var combatEventsContext = new CombatEventsContext();
            var commandQueue = new CommandQueue(combatEventsContext);

            var attackerA = new Unit("A", 5, 1, commandQueue, combatEventsContext);
            var targetB = new Unit("B", 5, 0, commandQueue, combatEventsContext);
            var defenderE = new Unit("E", 5, 1, commandQueue, combatEventsContext);
            defenderE.AddDefenderStatusEffect();
            
            attackerA.GetCombatFeature<IDamageable>().DealDamage(targetB);
            
            
            // Anything above can be changed, but the result must be correct:
            var result = commandQueue.CalcResult();
            Assert.AreEqual(3, result.Count); 
            Assert.AreEqual(new TryAttackCommand(attackerA, targetB, 0), result[0]);
            Assert.AreEqual(new DefendCommand(defenderE, targetB, 0), result[1]);
            Assert.AreEqual(new AttackCommand(attackerA, defenderE, attackerA.Damage, 0), result[2]);
        }
    }
}
