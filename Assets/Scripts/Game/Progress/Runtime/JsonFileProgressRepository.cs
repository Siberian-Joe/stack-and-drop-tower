using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Progress.Contracts;
using Game.Progress.Data;
using UnityEngine;

namespace Game.Progress.Runtime
{
    public sealed class JsonFileProgressRepository : IProgressRepository
    {
        private const int CurrentSchema = 1;
        private readonly string _path;

        public JsonFileProgressRepository(string fileName = "progress.json") =>
            _path = Path.Combine(Application.persistentDataPath, fileName);

        public async UniTask<GameProgressSnapshot> LoadAsync(CancellationToken token)
        {
            if (File.Exists(_path) == false)
                return CreateDefault();

            await UniTask.SwitchToThreadPool();
            try
            {
                token.ThrowIfCancellationRequested();

                var json = await File.ReadAllTextAsync(_path, token);
                if (string.IsNullOrWhiteSpace(json))
                    return CreateDefault();

                var data = JsonUtility.FromJson<GameProgressData>(json);
                if (data == null)
                    return CreateDefault();

                // TODO: migrations data.schemaVersion -> CurrentSchema

                return GameProgressMapping.FromData(data);
            }
            finally
            {
                await UniTask.SwitchToMainThread();
            }
        }

        public async UniTask SaveAsync(GameProgressSnapshot progress, CancellationToken token)
        {
            var data = GameProgressMapping.ToData(progress);
            data.SchemaVersion = CurrentSchema;

            var json = JsonUtility.ToJson(data);

            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var temp = _path + ".tmp";

            await UniTask.SwitchToThreadPool();
            try
            {
                token.ThrowIfCancellationRequested();

                await File.WriteAllTextAsync(temp, json, token);
                File.Copy(temp, _path, overwrite: true);
                File.Delete(temp);
            }
            finally
            {
                await UniTask.SwitchToMainThread();
            }
        }

        private static GameProgressSnapshot CreateDefault()
        {
            var tower = new TowerProgressSnapshot(Array.AsReadOnly(Array.Empty<CubePlacementSnapshot>()));
            var levels = new LevelProgressSnapshot(0, 0);
            return new GameProgressSnapshot(CurrentSchema, tower, levels);
        }
    }
}