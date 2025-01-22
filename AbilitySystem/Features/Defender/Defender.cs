using System.Collections.Generic;

namespace AbilitySystem
{
    public sealed class Defender : IStatusEffect
    {
        private readonly CommandQueue _commandQueue;
        private IUnit _unit = null!;

        public Defender(CommandQueue commandQueue)
        {
            _commandQueue = commandQueue;
        }

        public void Init(IUnit unit, List<IUnit> units)
        {
            _unit = unit;
            foreach (var u in units)
            {
                u.SubscribeToPreDamage(OnPreDamage);
            }
        }

        private bool OnPreDamage(IUnit source, IUnit target, int damage)
        {
            if (_unit == source || _unit == target)
            {
                return false;
            }

            _commandQueue.Add(new TryAttackCommand(source, target));
            _commandQueue.Add(new DefendCommand(_unit, target));
            source.DealDamage(_unit);
            return true;
        }
    }
}