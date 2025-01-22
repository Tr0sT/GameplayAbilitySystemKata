using System.Collections.Generic;
using System.Linq;

namespace AbilitySystem
{
    public static class DefenderExtensions
    {
        public static bool IsDefender(this IUnit unit)
        {
            return unit.StatusEffects.Any(s => s is Defender);
        }
        
        public static void AddDefenderStatusEffect(this IUnit unit, CommandQueue commandQueue, List<IUnit> units)
        {
            if (!unit.IsBully())
            {
                unit.AddStatusEffect(new Defender(commandQueue), units);
            }
        }
    }
}