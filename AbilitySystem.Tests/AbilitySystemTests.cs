using System.Collections.Generic;
using Nuclear.AbilitySystem;
using NUnit.Framework;

namespace AbilitySystemTests
{
    public sealed class AbilitySystemTests
    {
        [Test]
        public void DoubleStrikeWithOneBully_Full()
        {
            // Setup
            var attackerA = new Unit("A", 5, 1);
            var targetB = new Unit("B", 5, 0);
            var bullyC = new Unit("C", 5, 1);
            
            bullyC.AddBullyStatusEffect();
            attackerA.AddAbility(DoubleStrikeAbility.Create());
            
            // Ability execution
            var combatEventsContext = CombatEventBus.DeepCloneAndCreate(new() {attackerA, targetB, bullyC});
            var abilityContext = new AbilityContextHolder();
            var doubleStrikeAbility = combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IAbilitiesHolder>().Abilities[0];
            abilityContext.GetContext<ITimeAbilityContext>().NextTurn();
            
            Assert.AreEqual(true, doubleStrikeAbility.CanExecute(null!, null!, abilityContext));
            doubleStrikeAbility.Execute(
                attackerA.Id,
                targetB.Id,
                combatEventsContext,
                abilityContext);
            
            var result = combatEventsContext.CommandQueue.CalculateCommandQueue();
            
            // Tests
            Assert.AreEqual(6, result.Count); 
            Assert.Contains(new CreateProjectileCommand(attackerA.Id, targetB.Id, 1, 0), result);
            Assert.Contains(new AttackCommand(attackerA.Id, targetB.Id, attackerA.Damage, 1), result);
            Assert.Contains(new AttackCommand(bullyC.Id, targetB.Id, bullyC.Damage, 1), result);
            Assert.Contains(new CreateProjectileCommand(attackerA.Id, targetB.Id, 1, 1), result);
            Assert.Contains(new AttackCommand(attackerA.Id, targetB.Id, attackerA.Damage, 2), result);
            Assert.Contains(new AttackCommand(bullyC.Id, targetB.Id, bullyC.Damage, 2), result);
            
            Assert.AreEqual(false, doubleStrikeAbility.CanExecute(null!, null!, abilityContext));
            combatEventsContext.Dispose();
        }
        
        [Test]
        public void DoubleStrikeWithOneBully_OneShot()
        {
            var attackerA = new Unit("A", 5, 10);
            var targetB = new Unit("B", 5, 0);
            var bullyC = new Unit("C", 5, 1);
            bullyC.AddBullyStatusEffect();

            var combatEventsContext = CombatEventBus.DeepCloneAndCreate(new List<IUnit>{attackerA, targetB, bullyC});


            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);
            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);
            
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

            var combatEventsContext = CombatEventBus.DeepCloneAndCreate(new List<IUnit>{attackerA, targetB, bullyC, bullyD});


            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);
            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);


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
            
            var combatEventsContext = CombatEventBus.DeepCloneAndCreate(new List<IUnit>{attackerA, targetB, defenderE});


            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);
            
            
            // Anything above can be changed, but the result must be correct:
            var result = combatEventsContext.CommandQueue.CalculateCommandQueue();
            Assert.AreEqual(3, result.Count); 
            Assert.AreEqual(new TryAttackCommand(attackerA.Id, targetB.Id, 0), result[0]);
            Assert.AreEqual(new DefendCommand(defenderE.Id, targetB.Id, 0), result[1]);
            Assert.AreEqual(new AttackCommand(attackerA.Id, defenderE.Id, attackerA.Damage, 0), result[2]);
        }
        
        [Test]
        public void SingleStrikeWithOneBullyAndOneDefender()
        {
            var attackerA = new Unit("A", 5, 1);
            var targetB = new Unit("B", 5, 0);
            var bullyC = new Unit("C", 5, 1);
            var defenderD = new Unit("D", 5, 1);
            
            bullyC.AddBullyStatusEffect();
            defenderD.AddDefenderStatusEffect();
            
            var combatEventsContext = CombatEventBus.DeepCloneAndCreate(new List<IUnit> { attackerA, targetB, bullyC, defenderD });
            
            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);
            
            var result = combatEventsContext.CommandQueue.CalculateCommandQueue();
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(new TryAttackCommand(attackerA.Id, targetB.Id, 0), result[0]);
            Assert.AreEqual(new DefendCommand(defenderD.Id, targetB.Id, 0), result[1]);
            Assert.AreEqual(new AttackCommand(attackerA.Id, defenderD.Id, attackerA.Damage, 0), result[2]);
            Assert.AreEqual(new AttackCommand(bullyC.Id, defenderD.Id, bullyC.Damage, 0), result[3]);
            Assert.AreEqual(5, ((Unit)combatEventsContext.GetUnit(targetB.Id)).Health);
            Assert.AreEqual(3, ((Unit)combatEventsContext.GetUnit(defenderD.Id)).Health);
            Assert.AreEqual(5, defenderD.Health);
        }
    }
}
