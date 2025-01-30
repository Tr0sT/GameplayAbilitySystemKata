using System.Linq;

namespace AbilitySystem
{
    public static class DefenderExtensions
    {
        public static bool IsDefender(this IUnit unit)
        {
            return unit.GetCombatFeature<IStatusEffectsHolder>().StatusEffects.Any(s => s is Defender);
        }
        
        public static void AddDefenderStatusEffect(this IUnit unit)
        {
            if (!unit.IsDefender())
            {
                unit.GetCombatFeature<IStatusEffectsHolder>().AddStatusEffect(
                    new Defender(unit.Id));
            }
        }
    }
}