using System;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class PlatformSpawner : IPlatformSpawner
    {
        public event Action OnPlatformMissed;
        public event Action LastPlatformPlaced;
        private readonly Transform _platformParent;
        private readonly DiContainer _container;
        private readonly MovingPlatform _platformPrefab;
        private readonly FinishPlatform _finishPlatformPrefab;
        private readonly GameSettings _gameSettings;

        public IPlatform LastFinishPlatform { get; set; }

        private Vector3 _lastPlatformPosition;
        private bool _spawnRight = true;
        private bool _canSpawn = true;
        private int _spawnedPlatformsCount = 0;
        private int _maxPlatformsForCurrentLevel = 0;

        private readonly List<IPlatform> _spawnedPlatforms = new();
        private const int MaxPlatformCount = 30;

        public float ZStep { get; set; } = 2f;
        public float XStep { get; set; } = 3f;

        private MovingPlatform _lastSpawnedPlatform;

        public PlatformSpawner(DiContainer container, MovingPlatform platformPrefab,
            FinishPlatform finishPlatformPrefab, GameSettings gameSettings, Transform platformParent)
        {
            _container = container;
            _platformPrefab = platformPrefab;
            _finishPlatformPrefab = finishPlatformPrefab;
            _gameSettings = gameSettings;
            _platformParent = platformParent;

            _lastPlatformPosition = Vector3.zero;
        }

        public void DisableSpawning()
        {
            _canSpawn = false;
        }

        public void EnableSpawning()
        {
            _canSpawn = true;
        }

        public IPlatform SpawnInitial(Vector3? spawnPos = null)
        {
            var platform = _container.InstantiatePrefabForComponent<MovingPlatform>(
                _platformPrefab,
                spawnPos.GetValueOrDefault(Vector3.zero),
                Quaternion.identity,
                _platformParent
            );

            var scale = platform.transform.localScale;
            scale.x = _gameSettings.initialPlatformWidth / platform.GetComponent<BoxCollider>().size.x;
            platform.transform.localScale = scale;

            platform.Initialize(spawnPos.GetValueOrDefault(Vector3.zero), moveRight: true);
            platform.StopMoving();

            _lastPlatformPosition = spawnPos.GetValueOrDefault(Vector3.zero);
            _lastSpawnedPlatform = platform;

            RegisterPlatform(platform);
            return platform;
        }

        public IPlatform SpawnNext(float width)
        {
            if (!_canSpawn) return null;

            if (_spawnedPlatformsCount >= _maxPlatformsForCurrentLevel)
            {
                LastPlatformPlaced?.Invoke();
                return null;
            }

            var clampedWidth = Mathf.Max(_gameSettings.minPlatformWidth, width);
            var newZ = _lastPlatformPosition.z + ZStep;
            var newX = _spawnRight ? XStep : -XStep;
            var spawnPos = new Vector3(newX, 0f, newZ);

            var platform = _container.InstantiatePrefabForComponent<MovingPlatform>(
                _platformPrefab,
                spawnPos,
                Quaternion.identity,
                _platformParent
            );

            var scale = platform.transform.localScale;
            scale.x = clampedWidth / platform.GetComponent<BoxCollider>().size.x;
            platform.transform.localScale = scale;

            var moveRight = !_spawnRight;
            platform.Initialize(spawnPos, moveRight);
            platform.StartMoving();

            platform.SetTargetPlatform(_lastSpawnedPlatform);

            platform.OnFall += () => OnPlatformMissed?.Invoke();

            _lastPlatformPosition = spawnPos;
            _lastSpawnedPlatform = platform;
            _spawnRight = !_spawnRight;
            _spawnedPlatformsCount += 1;

            RegisterPlatform(platform);
            return platform;
        }

        public FinishPlatform SpawnFinishPlatform(Vector3 startPosition, int stepCount)
        {
            stepCount += 1;
            var zOffset = ZStep * stepCount + startPosition.z;

            var finish = _container.InstantiatePrefabForComponent<FinishPlatform>(
                _finishPlatformPrefab,
                new Vector3(0f, 0f, zOffset),
                Quaternion.identity,
                _platformParent);

            RegisterPlatform(finish);
            LastFinishPlatform = finish;
            return finish;
        }

        public FinishPlatform GetSecondLastFinishPlatform()
        {
            var count = 0;
            for (var i = _spawnedPlatforms.Count - 1; i >= 0; i--)
            {
                if (_spawnedPlatforms[i] is not FinishPlatform finish) continue;
                count += 1;
                if (count == 2)
                {
                    return finish;
                }
            }

            return null;
        }


        public void HandleFail()
        {
            if (LastFinishPlatform == null) return;

            var secondLastFinishPlatform = GetSecondLastFinishPlatform();

            if (secondLastFinishPlatform is null)
            {
                for (var i = _spawnedPlatforms.Count - 1 - 1; i >= 0; i--)
                {
                    if (_spawnedPlatforms[i] is MovingPlatform platform)
                    {
                        platform.FallOnFail();
                        _spawnedPlatforms.Remove(platform);
                    }
                }
                return;
            }
            
            var temp = new List<MovingPlatform>();
            foreach (var p in _spawnedPlatforms)
            {
                if (p is MovingPlatform platform && platform.transform.position.z > secondLastFinishPlatform.transform.position.z)
                {
                    temp.Add(platform);
                }
            }

            foreach (var platform in temp)
            {
                platform.FallOnFail();
                _spawnedPlatforms.Remove(platform);
            }
        }


        private void RegisterPlatform(IPlatform platform)
        {
            _spawnedPlatforms.Add(platform);

            if (_spawnedPlatforms.Count <= MaxPlatformCount) return;
            var toRemove = _spawnedPlatforms[0];
            (toRemove as MonoBehaviour)?.gameObject.SetActive(false);
            _spawnedPlatforms.RemoveAt(0);
        }


        public void SetStartPosition(Vector3 start, int platformsCount)
        {
            _spawnedPlatformsCount = 0;
            _lastPlatformPosition = start;
            _maxPlatformsForCurrentLevel = platformsCount;
        }
    }
}