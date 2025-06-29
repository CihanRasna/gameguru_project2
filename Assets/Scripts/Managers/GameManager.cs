using Gameplay;
using UnityEngine;

namespace Managers
{
    public class GameManager : IGameManager
    {
        private readonly ICharacter _character;
        private readonly IPlatformSpawner _spawner;
        private readonly AudioSource _audioSource;
        private readonly AudioClip _noteClip;

        private IPlatform _lastPlatform;
        private IPlatform _currentPlatform;
        private readonly float _tolerance = 0.1f;
        private int _perfectCombo = 0;

        public GameManager(ICharacter character, IPlatformSpawner spawner, AudioSource audioSource, AudioClip noteClip)
        {
            _character = character;
            _spawner = spawner;
            _audioSource = audioSource;
            _noteClip = noteClip;

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

            if (Mathf.Abs(deltaX) <= _tolerance)
            {
                _perfectCombo++;
                PlayNote(true);
            }
            else
            {
                _perfectCombo = 0;
                PlayNote(false);
                (_currentPlatform as MovingPlatform)?.CutPlatform(deltaX);
            }

            var targetPos = new Vector3(_currentPlatform.GetPosition().x, 1f, _currentPlatform.GetPosition().z);
            _character.MoveTo(targetPos);

            _lastPlatform = _currentPlatform;
            var newWidth = _lastPlatform.GetWidth();
            _currentPlatform = _spawner.SpawnNext(newWidth);
            _currentPlatform.StartMoving();
        }


        private void PlayNote(bool perfect)
        {
            _audioSource.pitch = perfect ? 1 + 0.1f * _perfectCombo : 1f;
            _audioSource.PlayOneShot(_noteClip);
        }
    }
}