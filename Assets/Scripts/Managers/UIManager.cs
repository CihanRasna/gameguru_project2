using System.Collections;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject failPanel;
        private GameManager _gameManager;

        public void Initialize(GameManager gameManager)
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