using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class CharacterController : MonoBehaviour, ICharacter
    {
        [Inject] private GameSettings _gameSettings;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private Vector3 _targetPosition;
        private bool _isFalling;
        private bool _isSucceed;

        private float _baseSpeed;
        private float _speedMultiplier = 1f;
        private float _currentSpeed;
        private float _maxSpeed;
        private float _distanceThreshold;

        private static readonly int Success = Animator.StringToHash("Success");
        private static readonly int Play = Animator.StringToHash("Play");

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
                _rigidbody ??= gameObject.AddComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();

            _rigidbody.isKinematic = true;
            _targetPosition = transform.position;

            _baseSpeed = _gameSettings.characterMoveSpeed;
            _currentSpeed = _baseSpeed;

            _maxSpeed = _gameSettings.characterMaxSpeed;
            _distanceThreshold = _gameSettings.characterDistanceThreshold;
        }

        private void Update()
        {
            if (_isFalling || _isSucceed) return;

            var position = transform.position;
            var distance = Vector3.Distance(position, _targetPosition);

            UpdateSpeedBasedOnDistance(distance);

            transform.position = distance < 0.1f
                ? _targetPosition
                : Vector3.MoveTowards(position, _targetPosition, _currentSpeed * Time.deltaTime);
        }

        public Transform GetTransform() => transform;

        public void SetTargetPosition(Vector3 target)
        {
            _targetPosition = target;
        }

        public void SetSpeedMultiplier(float comboMultiplier)
        {
            _speedMultiplier = 1f + 0.1f * comboMultiplier;
        }

        public void ResetSpeedMultiplier()
        {
            _speedMultiplier = 1f;
        }

        private void UpdateSpeedBasedOnDistance(float distance)
        {
            float targetSpeed;

            if (distance > _distanceThreshold)
            {
                var t = Mathf.InverseLerp(_distanceThreshold, _distanceThreshold * 2f, distance);
                targetSpeed = Mathf.Lerp(_baseSpeed, _maxSpeed, t);
            }
            else
            {
                targetSpeed = _baseSpeed;
            }

            _currentSpeed = targetSpeed * _speedMultiplier;
        }

        public void StopMoving()
        {
            _isFalling = true;
            _rigidbody.isKinematic = false;
        }

        public void CelebrateSuccess()
        {
            transform.DOMove(_targetPosition, 0.25f);
            _isSucceed = true;
            _animator.SetTrigger(Success);
        }

        public void GetReadyForNextLevel()
        {
            _isSucceed = false;
            _isFalling = false;
            
            ResetSpeedMultiplier();
            _animator.SetTrigger(Play);
            _rigidbody.isKinematic = true;
            
            _targetPosition = transform.position;
            _baseSpeed = _gameSettings.characterMoveSpeed;
            _currentSpeed = _baseSpeed;

            _maxSpeed = _gameSettings.characterMaxSpeed;
            _distanceThreshold = _gameSettings.characterDistanceThreshold;
        }
    }
}