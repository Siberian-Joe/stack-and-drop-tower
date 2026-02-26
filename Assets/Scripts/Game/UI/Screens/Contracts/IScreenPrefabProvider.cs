using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.UI.Screens.Contracts
{
    public interface IScreenPrefabProvider
    {
        UniTask<GameObject> LoadPrefabAsync<TView>(CancellationToken token = default)
            where TView : ScreenView;
    }
}