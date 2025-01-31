using System.Collections.Generic;

namespace Nuclear.AbilitySystem
{
    public static class DoubleStrikeAbility
    {
        public static IAbility Create()
        {
            return new Ability(new List<IAbilityAction>()
                {
                    new CreateProjectileAbilityAction(new("1"), new List<IAbilityAction>()
                    {
                        new DealDamageAbilityAction(AbilityActionTarget.FromSourceToTarget, 1)
                    }.AsReadOnly()),
                    new DelayAbilityAction(1, new List<IAbilityAction>()
                    {
                        new CreateProjectileAbilityAction(new("2"), new List<IAbilityAction>()
                        {
                            new DealDamageAbilityAction(AbilityActionTarget.FromSourceToTarget, 1)
                        }.AsReadOnly())
                    }.AsReadOnly())
                }.AsReadOnly(),
                new List<IAbilityCheck>()
                {
                    new CooldownAbilityCheck(2)
                }.AsReadOnly());
        }
    }
}