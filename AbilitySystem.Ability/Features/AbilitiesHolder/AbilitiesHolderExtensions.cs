namespace Nuclear.AbilitySystem
{
    public static class AbilitiesHolderExtensions
    {
        public static void AddAbility(this IUnit unit, IAbility ability)
        {
            unit.GetCombatFeature<IAbilitiesHolder>().AddAbility(ability);
        }
    }
}