using System;
using Gameplay;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class PlatformSpawner : IPlatformSpawner
    {
        public event Action OnPlatformMissed;
        private readonly DiContainer _container;
        private readonly MovingPlatform _platformPrefab;
        private readonly GameSettings _gameSettings;

        private Vector3 _lastPlatformPosition;
        private bool _spawnRight = true;
        private bool _canSpawn = true;
        private int _spawnedPlatformsCount = 0;
        private int _maxPlatformsForCurrentLevel = 0;

        public float ZStep { get; set; } = 2f;
        public float XStep { get; set; } = 3f;

        private MovingPlatform _lastSpawnedPlatform;

        public PlatformSpawner(DiContainer container, MovingPlatform platformPrefab, GameSettings gameSettings)
        {
            _container = container;
            _platformPrefab = platformPrefab;
            _gameSettings = gameSettings;

            _lastPlatformPosition = Vector3.zero;
        }
        
        public void DisableSpawning()
        {
            _canSpawn = false;
        }
        
        public IPlatform SpawnInitial()
        {
            var spawnPos = Vector3.zero;

            var platform = _container.InstantiatePrefabForComponent<MovingPlatform>(
                _platformPrefab,
                spawnPos,
                Quaternion.identity,
                null
            );

            var scale = platform.transform.localScale;
            scale.x = _gameSettings.initialPlatformWidth / platform.GetComponent<BoxCollider>().size.x;
            platform.transform.localScale = scale;

            platform.Initialize(spawnPos, moveRight: true);
            platform.StopMoving();

            _lastPlatformPosition = spawnPos;
            _lastSpawnedPlatform = platform;

            return platform;
        }

        public IPlatform SpawnNext(float width)
        {
            if (!_canSpawn) return null;
            
            if (_spawnedPlatformsCount >= _maxPlatformsForCurrentLevel) return null;
            
            var clampedWidth = Mathf.Max(_gameSettings.minPlatformWidth, width);
            var newZ = _lastPlatformPosition.z + ZStep;
            var newX = _spawnRight ? XStep : -XStep;
            var spawnPos = new Vector3(newX, 0f, newZ);

            var platform = _container.InstantiatePrefabForComponent<MovingPlatform>(
                _platformPrefab,
                spawnPos,
                Quaternion.identity,
                null
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

            return platform;
        }


        public void SetStartPosition(Vector3 start, int platformsCount)
        {
            _lastPlatformPosition = start;
            _maxPlatformsForCurrentLevel = platformsCount;
        }
    }
}
