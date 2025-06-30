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
        private readonly CameraManager _cameraManager;
        private readonly UIManager _uiManager;

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
            DiContainer container,
            CameraManager cameraManager,
            UIManager uiManager)
        {
            _character = character;
            _spawner = spawner;
            _gameSettings = gameSettings;
            _audioManager = audioManager;
            _inputHandler = inputHandler;
            _finishPrefab = finishPrefab;
            _container = container;
            _uiManager = uiManager;
            _cameraManager = cameraManager;

            GameState = GameState.Start;

            var stepCount = gameSettings.GetStepCountForLevel(CurrentLevel);
            _spawner.SetStartPosition(Vector3.zero, stepCount);
            _lastPlatform = _spawner.SpawnInitial();
            var newWidth = _lastPlatform.GetWidth();
            _currentPlatform = _spawner.SpawnNext(newWidth);
            _currentPlatform.StopMoving();
            SpawnFinishPlatformForLevel(stepCount);
        }

        [Inject]
        public void Init()
        {
            _spawner.OnPlatformMissed += GameOver;
            _spawner.LastPlatformPlaced += LastPlatformPlaced;
        }

        public void LastPlatformPlaced()
        {
            GameState = GameState.Success;
            _character.SetSpeedMultiplier(25f);
            _character.SetTargetPosition(_currentFinish.GetPosition());
        }

        public void OnPlayerTap()
        {
            if (GameState == GameState.Start)
            {
                GameState = GameState.Playing;
                _currentPlatform.StartMoving();
                _character.SetTargetPosition(_lastPlatform.GetPosition());
                return;
            }

            if (GameState is GameState.Success or GameState.Fail) return;
            if (_currentPlatform is null or { IsMoving: false }) return;

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

        private void SpawnFinishPlatformForLevel(int stepCount)
        {
            var pos = Vector3.zero;
            if (_lastPlatform is not null)
            {
                pos = _lastPlatform.GetPosition();
            }

            _currentFinish = (_spawner as PlatformSpawner)?.SpawnFinishPlatform(pos, stepCount);

            if (_currentFinish != null)
            {
                _currentFinish.OnPlayerReached += LevelComplete;
            }
        }

        public void GameOver()
        {
            if (GameState is GameState.Fail or GameState.Success) return;
            GameState = GameState.Fail;
            Debug.Log("FAIL GAME OVER");

            _cameraManager.LevelFailCamTransition();
            _uiManager.ShowFailPanelDelayed(1f);

            _spawner.DisableSpawning();
            _inputHandler.DisableInput();
            _character.StopMoving();

            var castedSpawner = (_spawner as PlatformSpawner);
            castedSpawner?.HandleFail();
        }

        public void RestartFromLastPlatform()
        {
            var lastFinish = _spawner.LastFinishPlatform;
            var secondLast = _spawner.GetSecondLastFinishPlatform();
            if (lastFinish == null) return;
            _spawner.EnableSpawning();
            _inputHandler.EnableInput();
            _character.SetTargetPositionInstant(secondLast != null ? lastFinish.GetPosition() : Vector3.zero);
            _character.GetReadyForNextLevel();
            RebuildLevel();
        }

        public void LevelComplete()
        {
            Debug.Log("LEVEL COMPLETE");
            if (GameState is not GameState.Success) return;
            GameState = GameState.Success;
            Debug.Log("LEVEL COMPLETE");

            _character.CelebrateSuccess();
            CurrentLevel += 1;
            _cameraManager.LevelCompleteCamTransition();
            _uiManager.ShowWinPanelDelayed(1f);
        }


        public void NextLevel()
        {
            GameState = GameState.Start;
            var stepCount = _gameSettings.GetStepCountForLevel(CurrentLevel);
            _uiManager.UpdateLevelText();
            _spawner.SetStartPosition(_currentFinish.transform.position, stepCount);
            var newWidth = _gameSettings.initialPlatformWidth;
            _lastPlatform = _currentFinish;
            _currentPlatform = _spawner.SpawnNext(newWidth);
            _currentPlatform.StopMoving();
            SpawnFinishPlatformForLevel(stepCount);
            _character.SetTargetPosition(_lastPlatform.GetPosition());
            _character.GetReadyForNextLevel();
            _cameraManager.LevelStartCamTransition();
        }

        public void RebuildLevel()
        {
            GameState = GameState.Start;
            var stepCount = _gameSettings.GetStepCountForLevel(CurrentLevel);
            var secondLast = _spawner.GetSecondLastFinishPlatform();
            _uiManager.UpdateLevelText();
            var newWidth = _gameSettings.initialPlatformWidth;
            if (!secondLast)
            {
                Debug.Log("HAS NOT SECOND LAST");
                _lastPlatform = _spawner.SpawnInitial();
                _spawner.SetStartPosition(_lastPlatform.GetPosition(), stepCount);
                _character.SetTargetPosition(_lastPlatform.GetPosition());
            }
            else
            {
                Debug.Log("HAS SECOND LAST");
                
                Debug.Log(secondLast.transform.position);
                _lastPlatform = secondLast;
                _spawner.SetStartPosition(_lastPlatform.GetPosition(), stepCount);
                _character.SetTargetPositionInstant(secondLast.GetPosition());
                if (secondLast == (FinishPlatform)_spawner.LastFinishPlatform)
                {
                    SpawnFinishPlatformForLevel(stepCount);
                }
            }

            _currentPlatform = _spawner.SpawnNext(newWidth);
            _currentPlatform.StopMoving();

            _character.GetReadyForNextLevel();
            _cameraManager.LevelStartCamTransition();
        }
    }
}