using Gameplay;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class PlatformSpawner : IPlatformSpawner
    {
        private readonly DiContainer _container;
        private readonly MovingPlatform _platformPrefab;

        private Vector3 _lastPlatformPosition;
        private bool _spawnRight = true;

        private const float ZStep = 2f;
        private const float XStep = 3f;

        public PlatformSpawner(DiContainer container, MovingPlatform platformPrefab)
        {
            _container = container;
            _platformPrefab = platformPrefab;
            _lastPlatformPosition = Vector3.zero;
        }

        public IPlatform SpawnInitial()
        {
            var spawnPos = new Vector3(0f, 0f, 0f);
            var platform = _container.InstantiatePrefabForComponent<MovingPlatform>(
                _platformPrefab,
                spawnPos,
                Quaternion.identity,
                null
            );

            _lastPlatformPosition = spawnPos;
            return platform;
        }

        public IPlatform SpawnNext(float width)
        {
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
            scale.x = width;
            platform.transform.localScale = scale;

            _lastPlatformPosition = spawnPos;
            _spawnRight = !_spawnRight;

            return platform;
        }


        public void SetStartPosition(Vector3 start)
        {
            _lastPlatformPosition = start;
        }
    }
}