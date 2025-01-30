namespace AbilitySystem
{
    public interface IAbilityCheck
    {
        bool CanExecute(IUnitId source, IUnitId? target, IAbilityContextHolder context);
        void Execute(IUnitId source, IUnitId? target, IAbilityContextHolder context);
        IAbilityCheck DeepClone();
    }
}