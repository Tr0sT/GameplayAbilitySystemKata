using System.Numerics;

namespace Nuclear.AbilitySystem
{
    public sealed class HasPositionCombatFeature : IHasPositionCombatFeature
    {
        public HasPositionCombatFeature(Vector2 position)
        {
            Position = position;
        }

        public Vector2 Position { get; }

        public void Subscribe(ICombatEventBus combatEventBus) { }
        public void UnSubscribe() { }

        public ICombatFeature DeepClone()
        {
            return new HasPositionCombatFeature(Position);
        }
    }
}