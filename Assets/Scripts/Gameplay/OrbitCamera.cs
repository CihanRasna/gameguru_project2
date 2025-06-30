using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class OrbitCamera : MonoBehaviour
    {
        public Transform target;
        public float rotationSpeed = 10f;

        [Inject]
        public void Initialize(ICharacter character)
        {
            target = character.GetTransform();
        }

        private void OnEnable()
        {
            if (target == null)
            {
                target = FindObjectOfType<CharacterController>().transform;
            }
        }

        private void Update()
        {
            if (!target) return;

            transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}