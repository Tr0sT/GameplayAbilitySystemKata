using System.Collections.Generic;

namespace AbilitySystem.ECS
{
    public sealed class Calculator
    {
        private readonly List<Unit> _units;
        private readonly CommandQueue _commandQueue;
        private readonly List<ICombatSystem> _systems;

        public Calculator(List<Unit> units, CommandQueue commandQueue)
        {
            _units = units;
            _commandQueue = commandQueue;
            
            _systems = new List<ICombatSystem>();
            _systems.Add(new DealDamageToDeadCombatSystem());
            _systems.Add(new DefendCombatSystem());
            _systems.Add(new DealDamageCombatSystem());
            _systems.Add(new BullyCombatSystem());
        }

        public void Calculate()
        {
            while (_commandQueue.IsDirty)
            {
                var isDirty = false;
                foreach (var system in _systems)
                {
                    isDirty = system.Process(_units, _commandQueue);
                    if (isDirty)
                    {
                        break;
                    }
                }

                if (!isDirty)
                {
                    _commandQueue.SetActiveCommandProcessed();
                }
            }
        }
    }
}