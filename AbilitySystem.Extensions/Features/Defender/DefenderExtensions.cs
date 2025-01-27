using System.Linq;

namespace AbilitySystem
{
    public static class DefenderExtensions
    {
        public static bool IsDefender(this IUnit unit)
        {
            return unit.GetCombatFeature<IStatusEffectsHolder>().StatusEffects.Any(s => s is Defender);
        }
        
        public static void AddDefenderStatusEffect(this IUnit unit, ICommandQueue commandQueue, ICombatEventBus combatEventBus)
        {
            if (!unit.IsBully())
            {
                unit.GetCombatFeature<IStatusEffectsHolder>().AddStatusEffect(
                    new Defender(unit, commandQueue, combatEventBus));
            }
        }
    }
}