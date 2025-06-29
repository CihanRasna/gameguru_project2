using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class CharacterController : MonoBehaviour, ICharacter
    {
        [Inject] private GameSettings _gameSettings;

        private Rigidbody _rigidbody;
        private Vector3 _targetPosition;
        private bool _isFalling = false;

        private float _baseSpeed;
        private float _speedMultiplier = 1f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
                _rigidbody = gameObject.AddComponent<Rigidbody>();

            _rigidbody.isKinematic = true;
            _targetPosition = transform.position;

            _baseSpeed = _gameSettings.characterMoveSpeed;
        }

        private void Update()
        {
            if (_isFalling) return;

            var position = transform.position;
            var distance = Vector3.Distance(transform.position, _targetPosition);

            if (distance < 0.1f)
            {
                transform.position = _targetPosition;
            }
            else
            {
                var speed = _baseSpeed * _speedMultiplier;
                transform.position = Vector3.MoveTowards(position, _targetPosition, speed * Time.deltaTime);
            }
        }

        public void SetTargetPosition(Vector3 target)
        {
            _targetPosition = target;
        }

        public void SetSpeedMultiplier(float comboMultiplier)
        {
            var speedMultiplier = 1f + 0.1f * comboMultiplier;
            _speedMultiplier = speedMultiplier;
        }

        public void ResetSpeedMultiplier()
        {
            _speedMultiplier = 1f;
        }

        public void Fall()
        {
            _isFalling = true;
            _rigidbody.isKinematic = false;
        }
    }
}