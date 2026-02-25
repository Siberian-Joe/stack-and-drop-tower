namespace Game.UI.Screens.Contracts
{
    public interface IScreenRootsRegistry
    {
        bool IsInitialized { get; }

        void Set(IScreenRoots instance);
    }
}