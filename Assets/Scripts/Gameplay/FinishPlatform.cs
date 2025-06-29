using System;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    [RequireComponent(typeof(BoxCollider))]
    public class FinishPlatform : MonoBehaviour
    {
        public event Action OnPlayerReached;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<ICharacter>(out _))
            {
                OnPlayerReached?.Invoke();
            }
        }
    }
}