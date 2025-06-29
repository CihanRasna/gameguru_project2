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
        void MoveTo(Vector3 targetPosition);
    }

    public interface IGameManager
    {
        void OnPlayerTap();
    }

    public interface IPlatformSpawner
    {
        IPlatform SpawnInitial();
        public IPlatform SpawnNext(float width);

        void SetStartPosition(Vector3 start);
    }
    
    public interface IAudioManager
    {
        void PlayNote(int perfectCombo);
        void PlayFailure();
    }

}