using System;
using System.Collections.Generic;
using Game.Progress.Contracts;
using Game.Progress.Runtime;
using Game.UI.BottomBar.Contracts;
using R3;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class TowerStackState : ITowerStackState, ITowerProgressState
    {
        private readonly List<TowerCubeState> _items = new();
        private readonly Subject<Unit> _changed = new();

        public int Count => _items.Count;
        public bool HasTop => _items.Count > 0;

        public TowerCubeState Top =>
            _items.Count > 0
                ? _items[^1]
                : throw new InvalidOperationException("Tower stack is empty");

        public Observable<Unit> Changed => _changed;

        public TowerProgressSnapshot Snapshot
        {
            get
            {
                var result = new CubePlacementSnapshot[_items.Count];

                for (var i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    var rect = item.Rect;

                    var z = rect ? rect.anchoredPosition3D.z : 0f;
                    var localPosition = new Vector3(item.Target.x, item.Target.y, z);

                    var localRotation = rect
                        ? rect.localRotation
                        : Quaternion.identity;

                    result[i] = new CubePlacementSnapshot(
                        item.ColorId,
                        localPosition,
                        localRotation);
                }

                return new TowerProgressSnapshot(Array.AsReadOnly(result));
            }
        }

        public int IndexOf(RectTransform rect)
        {
            for (var i = 0; i < _items.Count; i++)
            {
                if (_items[i].Rect == rect)
                    return i;
            }

            return -1;
        }

        public void Add(in TowerCubeState cube)
        {
            _items.Add(cube);
            _changed.OnNext(Unit.Default);
        }

        public TowerCubeState[] ExtractAtAndRemoveAbove(int index, out TowerCubeState extractedCube)
        {
            if (index < 0 || index >= _items.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            extractedCube = _items[index];

            var aboveCount = _items.Count - index - 1;
            var above = new TowerCubeState[aboveCount];

            for (var i = 0; i < aboveCount; i++)
                above[i] = _items[index + 1 + i];

            _items.RemoveRange(index, _items.Count - index);

            _changed.OnNext(Unit.Default);

            return above;
        }
    }
}