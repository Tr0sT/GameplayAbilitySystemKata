using System.Numerics;

namespace Nuclear.AbilitySystem
{
    public sealed class DistanceBetweenUnitsAbilityContext : IDistanceBetweenUnitsAbilityContext
    {
        private readonly ICombatEventBus _combatEventsContext;

        public DistanceBetweenUnitsAbilityContext(ICombatEventBus combatEventsContext)
        {
            _combatEventsContext = combatEventsContext;
        }

        public float GetDistanceBetween(IUnitId unitId1, IUnitId unitId2)
        {
            var unit1PositionFeature = _combatEventsContext.GetUnit(unitId1).GetCombatFeature<IHasPositionCombatFeature>();
            var unit2PositionFeature = _combatEventsContext.GetUnit(unitId2).GetCombatFeature<IHasPositionCombatFeature>();
            
            return Vector2.Distance(unit1PositionFeature.Position, unit2PositionFeature.Position);
        }

        public IAbilityContext DeepClone()
        {
            return new DistanceBetweenUnitsAbilityContext(_combatEventsContext);
        }
    }
}