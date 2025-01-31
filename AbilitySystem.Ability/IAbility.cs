using System.Collections.ObjectModel;
using System.Linq;

namespace Nuclear.AbilitySystem
{
    public interface IAbility
    {
        bool CanExecute(IUnitId source, IUnitId? target, IAbilityContextHolder abilityContext);
        void Execute(IUnitId source, IUnitId? target, ICombatEventBus context, IAbilityContextHolder abilityContext);
        IAbility DeepClone();
    }

    public class Ability : IAbility
    {
        private readonly ReadOnlyCollection<IAbilityAction> _abilityActions;
        private readonly ReadOnlyCollection<IAbilityCheck> _abilityChecks;

        public Ability(ReadOnlyCollection<IAbilityAction> abilityActions, 
            ReadOnlyCollection<IAbilityCheck> abilityChecks)
        {
            _abilityActions = abilityActions;
            _abilityChecks = abilityChecks;
        }

        public bool CanExecute(IUnitId source, IUnitId? target, IAbilityContextHolder abilityContext)
        {
            return _abilityChecks.All(a => a.CanExecute(source, target, abilityContext));
        }

        public void Execute(IUnitId source, IUnitId? target, ICombatEventBus context, IAbilityContextHolder abilityContext)
        {
            foreach (var abilityCheck in _abilityChecks)
            {
                abilityCheck.Execute(source, target, abilityContext);
            }
            foreach (var abilityAction in _abilityActions)
            {
                abilityAction.Execute(source, target, context, abilityContext);
            }
        }

        public IAbility DeepClone()
        {
            return new Ability(_abilityActions.Select(a => a.DeepClone()).ToList().AsReadOnly(),
                _abilityChecks.Select(a => a.DeepClone()).ToList().AsReadOnly());
        }
    }
}