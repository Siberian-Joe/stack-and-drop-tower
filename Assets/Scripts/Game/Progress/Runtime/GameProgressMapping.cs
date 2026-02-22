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
            var cubeArr = (data.tower?.cubes ?? new List<CubePlacementData>())
                .Select(x => new CubePlacementSnapshot(
                    x.colorId,
                    x.localPosition,
                    x.localRotation))
                .ToArray();

            var tower = new TowerProgressSnapshot(Array.AsReadOnly(cubeArr));

            var levels = new LevelProgressSnapshot(
                data.levels?.completedLevels ?? 0,
                data.levels?.currentLevelIndex ?? 0);

            return new GameProgressSnapshot(data.schemaVersion, tower, levels);
        }

        public static GameProgressData ToData(GameProgressSnapshot snapshot)
        {
            var data = new GameProgressData
            {
                schemaVersion = snapshot.SchemaVersion,
                tower = new TowerProgressData(),
                levels = new LevelProgressData
                {
                    completedLevels = snapshot.Levels.CompletedLevels,
                    currentLevelIndex = snapshot.Levels.CurrentLevelIndex,
                }
            };

            foreach (var cube in snapshot.Tower.Cubes)
            {
                data.tower.cubes.Add(new CubePlacementData
                {
                    colorId = cube.ColorId,
                    localPosition = cube.LocalPosition,
                    localRotation = cube.LocalRotation,
                });
            }

            return data;
        }
    }
}