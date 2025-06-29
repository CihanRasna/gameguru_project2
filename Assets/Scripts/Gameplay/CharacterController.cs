using UnityEngine;
using System.Collections;

namespace Gameplay
{
    public class CharacterController : MonoBehaviour, ICharacter
    {
        public float moveSpeed = 5f;

        public void MoveTo(Vector3 targetPosition)
        {
            StopAllCoroutines();
            StartCoroutine(MoveToRoutine(targetPosition));
        }

        private IEnumerator MoveToRoutine(Vector3 target)
        {
            var start = transform.position;
            var elapsed = 0f;
            var duration = 0.5f;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(start, target, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
        }
    }
}