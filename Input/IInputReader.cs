namespace ReiBrary.Input
{
    public interface IInputReader : IGameplayInputReader, IMenuInputReader
    {
        // TODO: OnDeviceLost, etc.
        public void DisableAllInput();
    }
}