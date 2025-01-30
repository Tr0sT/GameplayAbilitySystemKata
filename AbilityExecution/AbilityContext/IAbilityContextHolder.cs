namespace AbilitySystem
{
    public interface IAbilityContextHolder
    {
        T GetContext<T>() where T : IAbilityContext;
        IAbilityContextHolder DeepClone();
    }
}