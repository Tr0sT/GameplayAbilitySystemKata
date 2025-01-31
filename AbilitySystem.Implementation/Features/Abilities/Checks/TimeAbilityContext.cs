namespace Nuclear.AbilitySystem
{
    public sealed class TimeAbilityContext : ITimeAbilityContext
    {
        public TimeAbilityContext(int time)
        {
            Time = time;
        }

        public int Time { get; private set; }
        public void NextTurn()
        {
            Time++;
        }

        public IAbilityContext DeepClone()
        {
            return new TimeAbilityContext(Time);
        }
    }
}