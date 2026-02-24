namespace Game.UI.BottomBar.Contracts
{
    public interface IDropActionRuntime
    {
        BottomBarDragOrigin Origin { get; }

        bool TryExecuteHoleDrop();
        bool TryExecuteTowerPlacement();
        void ExecuteFailDrop();
    }
}