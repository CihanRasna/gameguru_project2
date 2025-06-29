using Gameplay;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class InputHandler : MonoBehaviour
    {
        private IGameManager _gameManager;

        [Inject]
        public void Construct(IGameManager gM)
        {
            _gameManager = gM;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _gameManager.OnPlayerTap();
            }
        }
    }
}