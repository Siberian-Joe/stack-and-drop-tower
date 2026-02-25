namespace Game.Config.Contracts
{
    public interface IGameConfigDefinition
    {
        IBottomBarConfigDefinition BottomBar { get; }
        ICubeDragConfigDefinition CubeDrag { get; }
    }
}