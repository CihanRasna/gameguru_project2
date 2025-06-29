using Gameplay;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class InputHandler : MonoBehaviour
    {
        private IGameManager _gameManager;
        private bool _inputEnabled = true;

        [Inject]
        public void Construct(IGameManager gM)
        {
            _gameManager = gM;
        }
        
        public void DisableInput()
        {
            _inputEnabled = false;
        }

        private void Update()
        {
            if (!_inputEnabled) return;
            if (Input.GetMouseButtonDown(0))
            {
                _gameManager.OnPlayerTap();
            }
        }
    }
}