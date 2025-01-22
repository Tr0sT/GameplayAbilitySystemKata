using System.Collections.Generic;

namespace AbilitySystem
{
    public interface IStatusEffect
    {
        void Init(IUnit targetUnit, List<IUnit> units);
    }
}