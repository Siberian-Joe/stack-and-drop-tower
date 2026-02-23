using UnityEngine;

namespace Game.Config.Contracts
{
    public interface ICubeColorDefinition
    {
        string Id { get; }
        Sprite Sprite { get; }
    }
}