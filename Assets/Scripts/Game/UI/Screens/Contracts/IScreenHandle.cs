using System;
using UnityEngine;

namespace Game.UI.Screens.Contracts
{
    public interface IScreenHandle
    {
        Type ViewType { get; }
        bool IsLoaded { get; }
        bool IsOpen { get; }

        void Open();
        void Close();
    }

    public interface IScreenHandle<TView> : IScreenHandle where TView : Component
    {
        TView View { get; }
        bool TryGetView(out TView view);
    }
}