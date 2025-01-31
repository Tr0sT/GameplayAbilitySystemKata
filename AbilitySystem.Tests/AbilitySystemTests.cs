using System.Collections.Generic;
using System.Numerics;
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
            var attackerA = new TestUnit("A", 5, 1, new Vector2(0, 0));
            var targetB = new TestUnit("B", 5, 0, new Vector2(0, 2));
            var bullyC = new TestUnit("C", 5, 1, new Vector2(0, 0));
            
            bullyC.AddBullyStatusEffect();
            attackerA.AddAbility(DoubleStrikeAbility.Create());
            
            // Ability execution
            var combatEventsContext = CombatEventBus.DeepCloneAndCreate(new() {attackerA, targetB, bullyC});
            var abilityContext = new AbilityContextHolder(combatEventsContext);
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
            Assert.Contains(new CreateProjectileCombatCommand(attackerA.Id, targetB.Id, 2, 0), result);
            Assert.Contains(new AttackCombatCommand(attackerA.Id, targetB.Id, attackerA.Damage, 2), result);
            Assert.Contains(new AttackCombatCommand(bullyC.Id, targetB.Id, bullyC.Damage, 2), result);
            Assert.Contains(new CreateProjectileCombatCommand(attackerA.Id, targetB.Id, 2, 1), result);
            Assert.Contains(new AttackCombatCommand(attackerA.Id, targetB.Id, attackerA.Damage, 3), result);
            Assert.Contains(new AttackCombatCommand(bullyC.Id, targetB.Id, bullyC.Damage, 3), result);
            
            Assert.AreEqual(false, doubleStrikeAbility.CanExecute(null!, null!, abilityContext));
            combatEventsContext.Dispose();
        }
        
        [Test]
        public void DoubleStrikeWithOneBully_OneShot()
        {
            var attackerA = new TestUnit("A", 5, 10, new Vector2(0, 0));
            var targetB = new TestUnit("B", 5, 0, new Vector2(0, 0));
            var bullyC = new TestUnit("C", 5, 1, new Vector2(0, 0));
            bullyC.AddBullyStatusEffect();

            var combatEventsContext = CombatEventBus.DeepCloneAndCreate(new List<IUnit>{attackerA, targetB, bullyC});


            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);
            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);
            
            // Anything above can be changed, but the result must be correct:
            var result = combatEventsContext.CommandQueue.CalculateCommandQueue();
            Assert.AreEqual(2, result.Count); 
            Assert.AreEqual(new AttackCombatCommand(attackerA.Id, targetB.Id, 5, 0), result[0]);
            Assert.AreEqual(new DeathCombatCommand(targetB.Id, 0), result[1]);
        }
        
        [Test]
        public void DoubleStrikeWithTwoBully_Full()
        {
            
            var attackerA = new TestUnit("A", 5, 1, new Vector2(0, 0));
            var targetB = new TestUnit("B", 5, 0, new Vector2(0, 0));
            var bullyC = new TestUnit("C", 5, 1, new Vector2(0, 0));
            
            var bullyD = new TestUnit("D", 5, 1, new Vector2(0, 0));
            bullyC.AddBullyStatusEffect();
            bullyD.AddBullyStatusEffect();

            var combatEventsContext = CombatEventBus.DeepCloneAndCreate(new List<IUnit>{attackerA, targetB, bullyC, bullyD});


            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);
            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);


            // Anything above can be changed, but the result must be correct:
            var result = combatEventsContext.CommandQueue.CalculateCommandQueue();
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual(new AttackCombatCommand(attackerA.Id, targetB.Id, attackerA.Damage, 0), result[0]); // 4 хп
            Assert.AreEqual(new AttackCombatCommand(bullyC.Id, targetB.Id, bullyC.Damage, 0), result[1]); // 3 хп
            Assert.AreEqual(new AttackCombatCommand(bullyD.Id, targetB.Id, bullyD.Damage, 0), result[2]); // 2 хп
            Assert.AreEqual(new AttackCombatCommand(bullyC.Id, targetB.Id, bullyC.Damage, 0), result[3]); // 1 хп
            Assert.AreEqual(new AttackCombatCommand(bullyD.Id, targetB.Id, bullyD.Damage, 0), result[4]); // 0 хп
            Assert.AreEqual(new DeathCombatCommand(targetB.Id, 0), result[5]);
        }
        
        [Test]
        public void SingleStrikeWithOneDefender_Full()
        {
            var attackerA = new TestUnit("A", 5, 1, new Vector2(0, 0));
            var targetB = new TestUnit("B", 5, 0, new Vector2(0, 0));
            var defenderE = new TestUnit("E", 5, 1, new Vector2(0, 0));
            defenderE.AddDefenderStatusEffect();
            
            var combatEventsContext = CombatEventBus.DeepCloneAndCreate(new List<IUnit>{attackerA, targetB, defenderE});


            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);
            
            
            // Anything above can be changed, but the result must be correct:
            var result = combatEventsContext.CommandQueue.CalculateCommandQueue();
            Assert.AreEqual(3, result.Count); 
            Assert.AreEqual(new TryAttackCombatCommand(attackerA.Id, targetB.Id, 0), result[0]);
            Assert.AreEqual(new DefendCombatCommand(defenderE.Id, targetB.Id, 0), result[1]);
            Assert.AreEqual(new AttackCombatCommand(attackerA.Id, defenderE.Id, attackerA.Damage, 0), result[2]);
        }
        
        [Test]
        public void SingleStrikeWithOneBullyAndOneDefender()
        {
            var attackerA = new TestUnit("A", 5, 1, new Vector2(0, 0));
            var targetB = new TestUnit("B", 5, 0, new Vector2(0, 0));
            var bullyC = new TestUnit("C", 5, 1, new Vector2(0, 0));
            var defenderD = new TestUnit("D", 5, 1, new Vector2(0, 0));
            
            bullyC.AddBullyStatusEffect();
            defenderD.AddDefenderStatusEffect();
            
            var combatEventsContext = CombatEventBus.DeepCloneAndCreate(new List<IUnit> { attackerA, targetB, bullyC, defenderD });
            
            combatEventsContext.GetUnit(attackerA.Id).GetCombatFeature<IDamageable>().DealDamage(combatEventsContext.GetUnit(targetB.Id), 1);
            
            var result = combatEventsContext.CommandQueue.CalculateCommandQueue();
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(new TryAttackCombatCommand(attackerA.Id, targetB.Id, 0), result[0]);
            Assert.AreEqual(new DefendCombatCommand(defenderD.Id, targetB.Id, 0), result[1]);
            Assert.AreEqual(new AttackCombatCommand(attackerA.Id, defenderD.Id, attackerA.Damage, 0), result[2]);
            Assert.AreEqual(new AttackCombatCommand(bullyC.Id, defenderD.Id, bullyC.Damage, 0), result[3]);
            Assert.AreEqual(5, ((TestUnit)combatEventsContext.GetUnit(targetB.Id)).Health);
            Assert.AreEqual(3, ((TestUnit)combatEventsContext.GetUnit(defenderD.Id)).Health);
            Assert.AreEqual(5, defenderD.Health);
        }
    }
}
