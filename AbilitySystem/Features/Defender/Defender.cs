namespace AbilitySystem
{
    public sealed class Defender : IStatusEffect
    {
        private IUnit _unit = null!;
        private CommandQueue _commandQueue = null!;


        public void Init(IUnit unit, CommandQueue commandQueue, ICombatEventsContext combatEventsContext)
        {
            _unit = unit;
            _commandQueue = commandQueue;
            combatEventsContext.SubscribeToPreDamage(OnPreDamage);
        }

        private bool OnPreDamage(IUnit source, IUnit target, int damage)
        {
            if (_unit == source || _unit == target)
            {
                return false;
            }

            _commandQueue.Add(new TryAttackCommand(source, target, _commandQueue.Time));
            _commandQueue.Add(new DefendCommand(_unit, target, _commandQueue.Time));
            source.DealDamage(_unit);
            return true;
        }
    }
}