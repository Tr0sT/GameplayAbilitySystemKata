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
            var commandQueue = new CommandQueue();

            var attackerA = new Unit("A", 5, 1, commandQueue);
            var targetB = new Unit("B", 5, 0, commandQueue);
            var bullyC = new Unit("C", 5, 1, commandQueue);
            
            bullyC.AddBullyStatusEffect(new List<IUnit> {attackerA, targetB, bullyC});

            attackerA.DealDamage(targetB);
            attackerA.DealDamage(targetB);
            
            // Anything above can be changed, but the result must be correct:
            var result = commandQueue.GetResult();
            Assert.AreEqual(4, result.Count); 
            Assert.AreEqual(new AttackCommand(attackerA, targetB, attackerA.Damage), result[0]);
            Assert.AreEqual(new AttackCommand(bullyC, targetB, bullyC.Damage), result[1]);
            Assert.AreEqual(new AttackCommand(attackerA, targetB, attackerA.Damage), result[2]);
            Assert.AreEqual(new AttackCommand(bullyC, targetB, bullyC.Damage), result[3]);
        }
        
        [Test]
        public void DoubleStrikeWithOneBully_OneShot()
        {
            var commandQueue = new CommandQueue();
            
            var attackerA = new Unit("A", 5, 10, commandQueue);
            var targetB = new Unit("B", 5, 0, commandQueue);
            var bullyC = new Unit("C", 5, 1, commandQueue);
            bullyC.AddBullyStatusEffect(new List<IUnit>{attackerA, targetB, bullyC});
            
            attackerA.DealDamage(targetB);
            attackerA.DealDamage(targetB);
            
            // Anything above can be changed, but the result must be correct:
            var result = commandQueue.GetResult();
            Assert.AreEqual(2, result.Count); 
            Assert.AreEqual(new AttackCommand(attackerA, targetB, 5), result[0]);
            Assert.AreEqual(new DeathCommand(targetB), result[1]);
        }
        
        [Test]
        public void DoubleStrikeWithTwoBully_Full()
        {
            var commandQueue = new CommandQueue();
            
            var attackerA = new Unit("A", 5, 1, commandQueue);
            var targetB = new Unit("B", 5, 0, commandQueue);
            var bullyC = new Unit("C", 5, 1, commandQueue);
            
            var bullyD = new Unit("D", 5, 1, commandQueue);
            bullyC.AddBullyStatusEffect(new List<IUnit>{attackerA, targetB, bullyC, bullyD});
            bullyD.AddBullyStatusEffect(new List<IUnit>{attackerA, targetB, bullyC, bullyD});
            
            attackerA.DealDamage(targetB);
            attackerA.DealDamage(targetB);

            // Anything above can be changed, but the result must be correct:
            var result = commandQueue.GetResult();
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual(new AttackCommand(attackerA, targetB, attackerA.Damage), result[0]); // 4 хп
            Assert.AreEqual(new AttackCommand(bullyC, targetB, bullyC.Damage), result[1]); // 3 хп
            Assert.AreEqual(new AttackCommand(bullyD, targetB, bullyD.Damage), result[2]); // 2 хп
            Assert.AreEqual(new AttackCommand(bullyC, targetB, bullyC.Damage), result[3]); // 1 хп
            Assert.AreEqual(new AttackCommand(bullyD, targetB, bullyD.Damage), result[4]); // 0 хп
            Assert.AreEqual(new DeathCommand(targetB), result[5]);
        }
        
        [Test]
        public void SingleStrikeWithOneDefender_Full()
        {
            var commandQueue = new CommandQueue();
            
            var attackerA = new Unit("A", 5, 1, commandQueue);
            var targetB = new Unit("B", 5, 0, commandQueue);
            var defenderE = new Unit("E", 5, 1, commandQueue);
            defenderE.AddDefenderStatusEffect(commandQueue, new List<IUnit>{attackerA, targetB, defenderE});
            
            attackerA.DealDamage(targetB);
            
            
            // Anything above can be changed, but the result must be correct:
            var result = commandQueue.GetResult();
            Assert.AreEqual(3, result.Count); 
            Assert.AreEqual(new TryAttackCommand(attackerA, targetB), result[0]);
            Assert.AreEqual(new DefendCommand(defenderE, targetB), result[1]);
            Assert.AreEqual(new AttackCommand(attackerA, defenderE, attackerA.Damage), result[2]);
        }
    }
}
