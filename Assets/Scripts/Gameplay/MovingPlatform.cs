using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class MovingPlatform : MonoBehaviour, IPlatform
    {
        public float MoveSpeed => gameSettings.moveSpeed;
        public float movementRange = 2.5f;
        private int _direction = 1;
        [SerializeField] private BoxCollider boxCollider;
        public bool IsMoving { get; private set; } = false;
        
        private GameSettings gameSettings;

        [Inject]
        public void Construct(GameSettings settings)
        {
            gameSettings = settings;
        }

        private void Start()
        {
            var pos = transform.position;
            pos.x = 0f;
            transform.position = pos;
        }

        public void StartMoving()
        {
            IsMoving = true;
        }

        public void StopMoving()
        {
            IsMoving = false;
        }

        private void Update()
        {
            if (!IsMoving) return;

            transform.position += Vector3.right * (_direction * MoveSpeed * Time.deltaTime);

            var x = transform.position.x;
            if (x < -movementRange || x > movementRange)
            {
                _direction *= -1;
                var clampedX = Mathf.Clamp(x, -movementRange, movementRange);
                transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
            }
        }

        public void MoveTo(Vector3 position)
        {
            transform.position = position;
        }

        public float GetWidth() => boxCollider.size.x * transform.localScale.x;


        public Vector3 GetPosition() => transform.position;

        public void CutPlatform(float deltaX)
        {
            var originalWidth = boxCollider.size.x * transform.localScale.x;
            var absDelta = Mathf.Abs(deltaX);
            var overlap = originalWidth - absDelta;

            if (overlap <= 0f)
            {
                Debug.Log("Tamamen dışta, platform düşmeli!");
                return;
            }

            var newWidth = overlap;

            var newScale = transform.localScale;
            newScale.x = newWidth / boxCollider.size.x;
            transform.localScale = newScale;

            var newPos = transform.position;
            newPos.x -= deltaX / 2f;
            transform.position = newPos;

            SpawnFallingPiece(deltaX, absDelta);
        }


        private void SpawnFallingPiece(float deltaX, float fallWidth)
        {
            var height = boxCollider.size.y * transform.localScale.y;
            var depth = boxCollider.size.z * transform.localScale.z;

            var fallingPart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fallingPart.transform.localScale = new Vector3(fallWidth, height, depth);

            var partPos = transform.position;
            partPos.x += deltaX > 0
                ? (fallWidth / 2f + transform.localScale.x * boxCollider.size.x / 2f)
                : -(fallWidth / 2f + transform.localScale.x * boxCollider.size.x / 2f);

            fallingPart.transform.position = partPos;
            fallingPart.AddComponent<Rigidbody>();

            Destroy(fallingPart, 3f);
        }
    }
}