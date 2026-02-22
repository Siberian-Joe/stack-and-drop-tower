using System.Collections.Generic;

namespace Game.Config.Contracts
{
    public interface IBottomBarConfigDefinition
    {
        int Count { get; }
        IReadOnlyList<ICubeColorDefinition> Colors { get; }
    }
}