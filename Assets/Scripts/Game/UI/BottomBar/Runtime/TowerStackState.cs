using System;
using System.Collections.Generic;
using Game.UI.BottomBar.Contracts;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class TowerStackState : ITowerStackState
    {
        private readonly List<TowerCubeState> _items = new();

        public int Count => _items.Count;
        public bool HasTop => _items.Count > 0;

        public TowerCubeState Top =>
            _items.Count > 0
                ? _items[_items.Count - 1]
                : throw new InvalidOperationException("Tower stack is empty");

        public int IndexOf(RectTransform rect)
        {
            for (var i = 0; i < _items.Count; i++)
            {
                if (_items[i].Rect == rect)
                    return i;
            }

            return -1;
        }

        public void Add(in TowerCubeState cube) => _items.Add(cube);

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

            return above;
        }
    }
}