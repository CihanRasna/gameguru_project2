using UnityEngine;
using Zenject;
using System;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class MovingPlatform : MonoBehaviour, IPlatform
    {
        public float MoveSpeed => _gameSettings.moveSpeed;
        public float movementRange = 10f;
        private int _direction;
        [SerializeField] private BoxCollider boxCollider;
        private Renderer _rend;
        private MaterialPropertyBlock _propBlock;
        private Color _currentColor;
        public bool IsMoving { get; private set; } = false;

        private GameSettings _gameSettings;
        private IPlatform _targetPlatform;

        public event Action OnFall;

        [Inject]
        public void Construct(GameSettings settings)
        {
            _gameSettings = settings;
        }

        public void SetTargetPlatform(IPlatform target)
        {
            _targetPlatform = target;
        }

        public void Initialize(Vector3 startPos, bool moveRight)
        {
            _direction = moveRight ? 1 : -1;
            transform.position = startPos;
            SetColor(GetRandomColor());
        }

        private void Start()
        {
            _rend ??= GetComponent<Renderer>();
            _propBlock ??= new MaterialPropertyBlock();
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

            float previousX = transform.position.x;

            transform.position += Vector3.right * (_direction * MoveSpeed * Time.deltaTime);

            float currentX = transform.position.x;

            if ((_direction > 0 && currentX > previousX) || (_direction < 0 && currentX < previousX))
            {
                CheckIfMissedTarget();
            }
        }

        private void CheckIfMissedTarget()
        {
            if (_targetPlatform == null) return;

            float baseX = _targetPlatform.GetPosition().x;
            float baseHalf = _targetPlatform.GetWidth() / 2f;

            float myX = GetPosition().x;
            float myHalf = GetWidth() / 2f;

            if (_direction > 0) // Sağ (pozitif) yönünde hareket
            {
                // Eğer platformun sol ucu, hedef platformun sağ ucunu geçtiyse düş
                if (myX - myHalf > baseX + baseHalf)
                    Fall();
            }
            else if (_direction < 0) // Sol (negatif) yönünde hareket
            {
                // Eğer platformun sağ ucu, hedef platformun sol ucunu geçtiyse düş
                if (myX + myHalf < baseX - baseHalf)
                    Fall();
            }
        }


        public void Fall()
        {
            Debug.Log("⬇ Platform kaçtı, düşüyor.");
            IsMoving = false;

            if (!TryGetComponent<Rigidbody>(out var rb))
                rb = gameObject.AddComponent<Rigidbody>();

            OnFall?.Invoke();
        }

        private Color GetRandomColor()
        {
            if (_gameSettings.platformColors == null || _gameSettings.platformColors.Count == 0)
                return Color.white;

            var index = Random.Range(0, _gameSettings.platformColors.Count);
            return _gameSettings.platformColors[index];
        }

        public void SetColor(Color color)
        {
            _currentColor = color;

            if (_rend == null)
                _rend = GetComponent<Renderer>();

            if (_propBlock == null)
                _propBlock = new MaterialPropertyBlock();

            _rend.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_BaseColor", _currentColor);
            _rend.SetPropertyBlock(_propBlock);
        }

        private void SetColorToPart(GameObject part, Color color)
        {
            if (!part.TryGetComponent(out Renderer partRenderer)) return;
            var pb = new MaterialPropertyBlock();
            partRenderer.GetPropertyBlock(pb);
            pb.SetColor("_BaseColor", color);
            partRenderer.SetPropertyBlock(pb);
        }

        public void MoveTo(Vector3 position)
        {
            transform.position = position;
        }

        public void MatchPerfect()
        {
            transform.DOMoveX(_targetPlatform.GetPosition().x, 0.1f);
            PunchScale();
        }

        public void PunchScale()
        {
            transform.DOPunchScale(Vector3.one * 0.33f, 0.05f).OnComplete((() =>
            {
                (_targetPlatform as MovingPlatform)?.PunchScale();
            }));
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
                Fall();
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
            SetColorToPart(fallingPart, _currentColor);

            Destroy(fallingPart, 3f);
        }
    }
}