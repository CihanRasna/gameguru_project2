using Gameplay;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class GameManager : IGameManager
    {
        private readonly FinishPlatform _finishPrefab;
        private readonly ICharacter _character;
        private readonly IPlatformSpawner _spawner;
        private readonly IAudioManager _audioManager;
        private readonly GameSettings _gameSettings;
        private readonly DiContainer _container;
        private readonly InputHandler _inputHandler;

        private IPlatform _lastPlatform;
        private IPlatform _currentPlatform;
        private FinishPlatform _currentFinish;
        private float Tolerance => _gameSettings.perfectTolerance;
        private int _perfectCombo = 0;
        public GameState GameState { get; private set; } = GameState.Start;

        public int CurrentLevel
        {
            get => PlayerPrefs.GetInt("Level", 0);
            set => PlayerPrefs.SetInt("Level", value);
        }

        [Inject]
        public GameManager(
            ICharacter character,
            IPlatformSpawner spawner,
            IAudioManager audioManager,
            GameSettings gameSettings,
            InputHandler inputHandler,
            FinishPlatform finishPrefab,
            DiContainer container)
        {
            _character = character;
            _spawner = spawner;
            _gameSettings = gameSettings;
            _audioManager = audioManager;
            _inputHandler = inputHandler;
            _finishPrefab = finishPrefab;
            _container = container;

            GameState = GameState.Start;

            _spawner.SetStartPosition(Vector3.zero, _gameSettings.GetStepCountForLevel(CurrentLevel));

            _lastPlatform = spawner.SpawnInitial();
            var newWidth = _lastPlatform.GetWidth();
            _currentPlatform = spawner.SpawnNext(newWidth);
            _currentPlatform?.StartMoving();

            SpawnFinishPlatformForLevel(CurrentLevel);

            _character.SetTargetPosition(_lastPlatform.GetPosition());
        }

        [Inject]
        public void Init()
        {
            _spawner.OnPlatformMissed += GameOver;
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
                _currentPlatform.MatchPerfect();
                _character.SetSpeedMultiplier(_perfectCombo);
            }
            else
            {
                _perfectCombo = 0;
                _audioManager.PlayFailure();
                (_currentPlatform as MovingPlatform)?.CutPlatform(deltaX);
                _character.ResetSpeedMultiplier();
            }

            var targetPos = new Vector3(_currentPlatform.GetPosition().x, 1f, _currentPlatform.GetPosition().z);
            _character.SetTargetPosition(targetPos);

            _lastPlatform = _currentPlatform;
            var newWidth = _lastPlatform.GetWidth();
            _currentPlatform = _spawner.SpawnNext(newWidth);
            _currentPlatform?.StartMoving();
        }

        private void SpawnFinishPlatformForLevel(int level)
        {
            if (_currentFinish != null)
                Object.Destroy(_currentFinish.gameObject);

            var zStep = (_spawner as PlatformSpawner).ZStep;
            
            var stepCount = _gameSettings.GetStepCountForLevel(level);
            var beginPosZ = 0f;
            if (_currentFinish)
            {
                beginPosZ = _currentFinish.transform.position.z;
            }
            var zOffset = stepCount * zStep + beginPosZ;
            
            _currentFinish = _container.InstantiatePrefabForComponent<FinishPlatform>(
                _finishPrefab,
                new Vector3(0f, 0f, zOffset),
                Quaternion.identity,
                null);

            _currentFinish.OnPlayerReached += LevelComplete;
        }

        public void GameOver()
        {
            if (GameState is not GameState.Fail) return;
            GameState = GameState.Fail;

            _spawner.DisableSpawning();
            _inputHandler.DisableInput();
            _character.StopMoving();
        }

        public void LevelComplete()
        {
            if (GameState is not GameState.Success) return;

            GameState = GameState.Success;

            _character.StopMoving();
            CurrentLevel += 1;
            // UI aç, skor hesapla vs.
        }

        public void NextLevel()
        {
            _spawner.SetStartPosition(_currentFinish.transform.position,_gameSettings.GetStepCountForLevel(CurrentLevel));
            SpawnFinishPlatformForLevel(CurrentLevel);
            var newWidth = _gameSettings.initialPlatformWidth;
            _currentPlatform = _spawner.SpawnNext(newWidth);
            _currentPlatform.StartMoving();
            _character.SetTargetPosition(_lastPlatform.GetPosition());
        }
    }
}