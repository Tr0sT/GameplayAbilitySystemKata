using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbilitySystem
{
    public interface IAbilitiesHolder : ICombatFeature
    {
        void AddAbility(IAbility ability);
        ReadOnlyCollection<IAbility> Abilities { get; }
    }

    public sealed class AbilitiesHolder : IAbilitiesHolder
    {
        private readonly List<IAbility> _abilities = new();
        
        public void Subscribe(ICombatEventBus combatEventBus)
        {
        }

        public void UnSubscribe()
        {
        }

        public void AddAbility(IAbility ability)
        {
            _abilities.Add(ability);
        }

        public ReadOnlyCollection<IAbility> Abilities => _abilities.AsReadOnly();

        public ICombatFeature DeepClone()
        {
            var result = new AbilitiesHolder();
            foreach (var ability in _abilities)
            {
                result._abilities.Add(ability.DeepClone());
            }
            return result;
        }
    }
}