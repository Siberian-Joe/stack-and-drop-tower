using UnityEngine;

namespace Game.Config.Contracts
{
    public interface ICubeColorDefinition
    {
        string Id { get; }
        Color32 Color { get; }
        string localizationKey { get; }
    }
}