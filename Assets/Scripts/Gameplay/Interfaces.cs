using System;
using UnityEngine;

namespace Gameplay
{
    public interface IPlatform
    {
        Vector3 GetPosition();
        void StopMoving();
        void StartMoving();
        bool IsMoving { get; }
        void CutPlatform(float deltaX);
        float GetWidth();
        void MoveTo(Vector3 position);
        void MatchPerfect();
    }


    public interface ICharacter
    {
        void SetTargetPosition(Vector3 targetPosition);
        void SetSpeedMultiplier(float multiplier);
        void ResetSpeedMultiplier();
        void StopMoving();
        void CelebrateSuccess();
        void GetReadyForNextLevel();
    }

    public interface IGameManager
    {
        void OnPlayerTap();
        void LevelComplete();
        void GameOver();
        void NextLevel();
        GameState GameState { get; }
        int CurrentLevel { get; }
    }

    public interface IPlatformSpawner
    {
        IPlatform SpawnInitial(Vector3? spawnPos = null);
        public IPlatform SpawnNext(float width);
        void SetStartPosition(Vector3 start, int platformsCount);
        void DisableSpawning();
        event Action OnPlatformMissed;
        event Action LastPlatformPlaced;
    }
    
    public interface IAudioManager
    {
        void PlayNote(int perfectCombo);
        void PlayFailure();
    }

}