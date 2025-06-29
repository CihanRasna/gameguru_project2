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
    }

    public interface IGameManager
    {
        void OnPlayerTap();
        void LevelComplete();
        void GameOver();
        GameState GameState { get; }
    }

    public interface IPlatformSpawner
    {
        IPlatform SpawnInitial();
        public IPlatform SpawnNext(float width);
        void SetStartPosition(Vector3 start, int platformsCount);
        void DisableSpawning();
        event Action OnPlatformMissed;
    }
    
    public interface IAudioManager
    {
        void PlayNote(int perfectCombo);
        void PlayFailure();
    }

}