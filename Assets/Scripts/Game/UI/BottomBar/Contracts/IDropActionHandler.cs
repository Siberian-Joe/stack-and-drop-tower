namespace Game.UI.BottomBar.Contracts
{
    public interface IDropActionHandler
    {
        int Priority { get; }
        bool TryExecute();
    }
}