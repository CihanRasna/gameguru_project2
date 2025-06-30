using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class OrbitCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 rotateOffset = new(0, -8, -10f);
        [SerializeField] private float rotationSpeed = 10f;
        private float _angle;
        private Transform target;

        [Inject]
        public void Initialize(ICharacter character)
        {
            target = character.GetTransform();
        }

        private void OnEnable()
        {
            if (target == null)
            {
                _angle = 0f;
                target = FindObjectOfType<CharacterController>().transform;
                var offsetRotated = Quaternion.Euler(0, _angle, 0) * rotateOffset;
                transform.position = target.position + offsetRotated;
            }
        }

        private void Update()
        {
            if (!target) return;

            _angle += rotationSpeed * Time.deltaTime;

            var offsetRotated = Quaternion.Euler(0, _angle, 0) * rotateOffset;
            transform.position = target.position + offsetRotated;
        }
    }
}