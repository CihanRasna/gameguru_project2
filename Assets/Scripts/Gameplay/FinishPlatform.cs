using System;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    [RequireComponent(typeof(BoxCollider))]
    public class FinishPlatform : MonoBehaviour, IPlatform
    {
        public event Action OnPlayerReached;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<ICharacter>(out _))
            {
                OnPlayerReached?.Invoke();
            }
        }

        public void StopMoving()
        {
        }

        public void StartMoving()
        {
        }

        public bool IsMoving { get; }

        public void CutPlatform(float deltaX)
        {
        }

        public float GetWidth() => 0f;

        public Vector3 GetPosition() => transform.position;

        public void MoveTo(Vector3 position)
        {
        }

        public void MatchPerfect()
        {
        }
    }
}