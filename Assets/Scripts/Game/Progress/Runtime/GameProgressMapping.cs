using System;
using System.Collections.Generic;
using System.Linq;
using Game.Progress.Data;

namespace Game.Progress.Runtime
{
    public static class GameProgressMapping
    {
        public static GameProgressSnapshot FromData(GameProgressData data)
        {
            var cubeArr = (data.Tower?.Cubes ?? new List<CubePlacementData>())
                .Select(x => new CubePlacementSnapshot(
                    x.ColorId,
                    x.LocalPosition,
                    x.LocalRotation))
                .ToArray();

            var tower = new TowerProgressSnapshot(Array.AsReadOnly(cubeArr));

            var levels = new LevelProgressSnapshot(
                data.Levels?.CompletedLevels ?? 0,
                data.Levels?.CurrentLevelIndex ?? 0);

            return new GameProgressSnapshot(data.SchemaVersion, tower, levels);
        }

        public static GameProgressData ToData(GameProgressSnapshot snapshot)
        {
            var data = new GameProgressData
            {
                SchemaVersion = snapshot.SchemaVersion,
                Tower = new TowerProgressData(),
                Levels = new LevelProgressData
                {
                    CompletedLevels = snapshot.Levels.CompletedLevels,
                    CurrentLevelIndex = snapshot.Levels.CurrentLevelIndex,
                }
            };

            foreach (var cube in snapshot.Tower.Cubes)
            {
                data.Tower.Cubes.Add(new CubePlacementData
                {
                    ColorId = cube.ColorId,
                    LocalPosition = cube.LocalPosition,
                    LocalRotation = cube.LocalRotation,
                });
            }

            return data;
        }
    }
}