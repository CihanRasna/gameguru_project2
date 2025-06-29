using Gameplay;
using UnityEngine;

namespace Managers
{
    public class GameManager : IGameManager
    {
        private readonly ICharacter _character;
        private readonly IPlatformSpawner _spawner;
        private readonly IAudioManager _audioManager;


        private IPlatform _lastPlatform;
        private IPlatform _currentPlatform;
        private readonly GameSettings _gameSettings;
        private float Tolerance => _gameSettings.perfectTolerance;
        private int _perfectCombo = 0;

        public GameManager(ICharacter character, IPlatformSpawner spawner, IAudioManager audioManager, GameSettings gameSettings)
        {
            _character = character;
            _spawner = spawner;
            _gameSettings = gameSettings;
            _audioManager = audioManager;

            _lastPlatform = spawner.SpawnInitial();
            var newWidth = _lastPlatform.GetWidth();
            _currentPlatform = spawner.SpawnNext(newWidth);
            _currentPlatform.StartMoving();

            character.MoveTo(_lastPlatform.GetPosition());
        }

        public void OnPlayerTap()
        {
            if (!_currentPlatform.IsMoving) return;

            _currentPlatform.StopMoving();

            var deltaX = _currentPlatform.GetPosition().x - _lastPlatform.GetPosition().x;

            if (Mathf.Abs(deltaX) <= Tolerance)
            {
                _perfectCombo += 1;
                _audioManager.PlayNote(_perfectCombo);
            }
            else
            {
                _perfectCombo = 0;
                _audioManager.PlayFailure();
                (_currentPlatform as MovingPlatform)?.CutPlatform(deltaX);
            }

            var targetPos = new Vector3(_currentPlatform.GetPosition().x, 1f, _currentPlatform.GetPosition().z);
            _character.MoveTo(targetPos);

            _lastPlatform = _currentPlatform;
            var newWidth = _lastPlatform.GetWidth();
            _currentPlatform = _spawner.SpawnNext(newWidth);
            _currentPlatform.StartMoving();
        }
    }
}