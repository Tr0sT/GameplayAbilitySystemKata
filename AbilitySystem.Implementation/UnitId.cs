namespace Nuclear.AbilitySystem
{
    public sealed record UnitId(string Value) : IUnitId
    {
        public bool Equals(IUnitId other) => other is UnitId otherId && this == otherId;
    }
}