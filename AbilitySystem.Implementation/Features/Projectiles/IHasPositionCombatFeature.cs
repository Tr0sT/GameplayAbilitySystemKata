using System.Numerics;

namespace Nuclear.AbilitySystem
{
    public interface IHasPositionCombatFeature : ICombatFeature
    {
        Vector2 Position { get; }
    }
}