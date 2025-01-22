using NUnit.Framework;
using AbilitySystem;
using AbilitySystem.ECS;

namespace AbilitySystemTests
{
    public sealed class AbilitySystemTests
    {
        [Test]
        public void DoubleStrikeWithOneBully_Full()
        {
            var commandQueue = new CommandQueue();

            var attackerA = new Unit("A", 5, 1);
            var targetB = new Unit("B", 5, 0);
            var bullyC = new Unit("C", 5, 1);
            
            bullyC.AddBullyStatusEffect();
            
            commandQueue.Add(new AttackCommand(attackerA, targetB, attackerA.Damage));
            commandQueue.Add(new AttackCommand(attackerA, targetB, attackerA.Damage));

            var world = new Calculator(new() {attackerA, targetB, bullyC}, commandQueue);
            world.Calculate();
            
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
            
            var attackerA = new Unit("A", 5, 10);
            var targetB = new Unit("B", 5, 0);
            var bullyC = new Unit("C", 5, 1);
            bullyC.AddBullyStatusEffect();
            
            commandQueue.Add(new AttackCommand(attackerA, targetB, attackerA.Damage));
            commandQueue.Add(new AttackCommand(attackerA, targetB, attackerA.Damage));
            
            var world = new Calculator(new() {attackerA, targetB, bullyC}, commandQueue);
            world.Calculate();
            
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
            
            var attackerA = new Unit("A", 5, 1);
            var targetB = new Unit("B", 5, 0);
            var bullyC = new Unit("C", 5, 1);
            bullyC.AddBullyStatusEffect();
            
            var bullyD = new Unit("D", 5, 1);
            bullyD.AddBullyStatusEffect();
            
            commandQueue.Add(new AttackCommand(attackerA, targetB, attackerA.Damage));
            commandQueue.Add(new AttackCommand(attackerA, targetB, attackerA.Damage));
            
            var world = new Calculator(new() {attackerA, targetB, bullyC, bullyD}, commandQueue);
            world.Calculate();

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
            
            var attackerA = new Unit("A", 5, 1);
            var targetB = new Unit("B", 5, 0);
            var defenderE = new Unit("E", 5, 1);
            defenderE.AddDefenderStatusEffect();
            
            commandQueue.Add(new AttackCommand(attackerA, targetB, attackerA.Damage));
            
            var world = new Calculator(new() {attackerA, targetB, defenderE}, commandQueue);
            world.Calculate();
            
            // Anything above can be changed, but the result must be correct:
            var result = commandQueue.GetResult();
            Assert.AreEqual(3, result.Count); 
            Assert.AreEqual(new TryAttackCommand(attackerA, targetB), result[0]);
            Assert.AreEqual(new DefendCommand(defenderE, targetB), result[1]);
            Assert.AreEqual(new AttackCommand(attackerA, defenderE, attackerA.Damage), result[2]);
        }
    }
}
