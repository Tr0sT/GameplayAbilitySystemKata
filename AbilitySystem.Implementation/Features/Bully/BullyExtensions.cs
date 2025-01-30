using System.Linq;

namespace AbilitySystem
{
    public static class BullyExtensions
    {
        public static bool IsBully(this IUnit unit)
        {
            return unit.GetCombatFeature<IStatusEffectsHolder>().StatusEffects.Any(s => s is Bully);
        }
        
        public static void AddBullyStatusEffect(this IUnit unit)
        {
            if (!unit.IsBully())
            {
                unit.GetCombatFeature<IStatusEffectsHolder>().AddStatusEffect(
                    new Bully(unit.Id));
            }
        }
    }
}