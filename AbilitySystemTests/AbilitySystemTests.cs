using System.Collections.Generic;
using NUnit.Framework;
using AbilitySystem;

namespace AbilitySystemTests
{
    public sealed class AbilitySystemTests
    {
        [Test]
        public void DoubleStrikeWithOneBully_Full()
        {
            var attackerA = new Unit("A", 5, 1);
            var targetB = new Unit("B", 5, 0);
            var bullyC = new Unit("C", 5, 1);
            
            bullyC.AddBullyStatusEffect();
            
            var combatEventsContext = new CombatEventBus(new() {attackerA, targetB, bullyC});
            var commandQueueExtensions = new CommandQueueExtensions(combatEventsContext);
            DoubleAttackUsingProjectiles(combatEventsContext, commandQueueExtensions, attackerA.Id, targetB.Id);
            
            // Anything above can be changed, but the result must be correct:
            var result = combatEventsContext.CommandQueue.CalculateCommandQueue();
            Assert.AreEqual(6, result.Count); 
            Assert.Contains(new CreateProjectileCommand(attackerA.Id, targetB.Id, 10, 0), result);
            Assert.Contains(new AttackCommand(attackerA.Id, targetB.Id, attackerA.Damage, 10), result);
            Assert.Contains(new AttackCommand(bullyC.Id, targetB.Id, bullyC.Damage, 10), result);
            Assert.Contains(new CreateProjectileCommand(attackerA.Id, targetB.Id, 20, 5), result);
            Assert.Contains(new AttackCommand(attackerA.Id, targetB.Id, attackerA.Damage, 25), result);
            Assert.Contains(new AttackCommand(bullyC.Id, targetB.Id, bullyC.Damage, 25), result);
        }
        

        private void DoubleAttackUsingProjectiles(CombatEventBus combatEventsContext,
            ICommandQueueExtensions commandQueueExtensions, IUnitId source, IUnitId target)
        {
            commandQueueExtensions.CreateProjectile(source, 
                target, 10, () =>
            {
                combatEventsContext.GetUnit(source).GetCombatFeature<IDamageable>()
                    .DealDamage(combatEventsContext.GetUnit(target));
            });
            commandQueueExtensions.Delay(5, () =>
            {
                commandQueueExtensions.CreateProjectile(source, 
                    target, 20, () =>
                {
                    combatEventsContext.GetUnit(source).GetCombatFeature<IDamageable>()
                        .DealDamage(combatEventsContext.GetUnit(target));
                });
            });
        }
        
        [Test]
        public void DoubleStrikeWithOneBully_OneShot()
        {
            var attackerA = new Unit("A", 5, 10);
            var targetB = new Unit("B", 5, 0);
            var bullyC = new Unit("C", 5, 1);
            bullyC.AddBullyStatusEffect();

            var combatEventsContext = new CombatEventBus(new List<IUnit>{attackerA, targetB, bullyC});


            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id));
            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id));
            
            // Anything above can be changed, but the result must be correct:
            var result = combatEventsContext.CommandQueue.CalculateCommandQueue();
            Assert.AreEqual(2, result.Count); 
            Assert.AreEqual(new AttackCommand(attackerA.Id, targetB.Id, 5, 0), result[0]);
            Assert.AreEqual(new DeathCommand(targetB.Id, 0), result[1]);
        }
        
        [Test]
        public void DoubleStrikeWithTwoBully_Full()
        {
            
            var attackerA = new Unit("A", 5, 1);
            var targetB = new Unit("B", 5, 0);
            var bullyC = new Unit("C", 5, 1);
            
            var bullyD = new Unit("D", 5, 1);
            bullyC.AddBullyStatusEffect();
            bullyD.AddBullyStatusEffect();

            var combatEventsContext = new CombatEventBus(new List<IUnit>{attackerA, targetB, bullyC, bullyD});


            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id));
            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id));


            // Anything above can be changed, but the result must be correct:
            var result = combatEventsContext.CommandQueue.CalculateCommandQueue();
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual(new AttackCommand(attackerA.Id, targetB.Id, attackerA.Damage, 0), result[0]); // 4 хп
            Assert.AreEqual(new AttackCommand(bullyC.Id, targetB.Id, bullyC.Damage, 0), result[1]); // 3 хп
            Assert.AreEqual(new AttackCommand(bullyD.Id, targetB.Id, bullyD.Damage, 0), result[2]); // 2 хп
            Assert.AreEqual(new AttackCommand(bullyC.Id, targetB.Id, bullyC.Damage, 0), result[3]); // 1 хп
            Assert.AreEqual(new AttackCommand(bullyD.Id, targetB.Id, bullyD.Damage, 0), result[4]); // 0 хп
            Assert.AreEqual(new DeathCommand(targetB.Id, 0), result[5]);
        }
        
        [Test]
        public void SingleStrikeWithOneDefender_Full()
        {
            
            var attackerA = new Unit("A", 5, 1);
            var targetB = new Unit("B", 5, 0);
            var defenderE = new Unit("E", 5, 1);
            defenderE.AddDefenderStatusEffect();
            
            var combatEventsContext = new CombatEventBus(new List<IUnit>{attackerA, targetB, defenderE});


            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id));
            
            
            // Anything above can be changed, but the result must be correct:
            var result = combatEventsContext.CommandQueue.CalculateCommandQueue();
            Assert.AreEqual(3, result.Count); 
            Assert.AreEqual(new TryAttackCommand(attackerA.Id, targetB.Id, 0), result[0]);
            Assert.AreEqual(new DefendCommand(defenderE.Id, targetB.Id, 0), result[1]);
            Assert.AreEqual(new AttackCommand(attackerA.Id, defenderE.Id, attackerA.Damage, 0), result[2]);
        }
    }
}
