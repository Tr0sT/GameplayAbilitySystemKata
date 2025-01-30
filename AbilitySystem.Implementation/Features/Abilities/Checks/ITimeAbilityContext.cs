namespace AbilitySystem
{
    public interface ITimeAbilityContext : IAbilityContext
    {
        int Time { get; }
        void NextTurn();
    }
}