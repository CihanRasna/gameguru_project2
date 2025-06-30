using System.Collections;
using Gameplay;
using TMPro;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject failPanel;
        private IGameManager _gameManager;

        [Inject]
        public void Initialize(IGameManager gameManager)
        {
            _gameManager = gameManager;
            UpdateLevelText();
        }

        public void UpdateLevelText()
        {
            var level = _gameManager.CurrentLevel + 1;
            levelText.text = $"Level {level.ToString()}";
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