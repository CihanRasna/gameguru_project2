using System.Collections;
using Gameplay;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject failPanel;
        private IGameManager _gameManager;

        [Inject]
        public void Initialize(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void ShowWinPanelDelayed(float delay)
        {
            StartCoroutine(ShowPanelAfterDelay(delay));
        }

        private IEnumerator ShowPanelAfterDelay(float delay)
        {
            Debug.Log("Win Panel Delayed");
            yield return new WaitForSeconds(delay);
            winPanel.SetActive(true);
        }

        public void OnNextLevelButtonClicked()
        {
            winPanel.SetActive(false);
            _gameManager.NextLevel();
        }
    }
}