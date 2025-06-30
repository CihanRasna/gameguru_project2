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
        
        public void ShowFailPanelDelayed(float delay)
        {
            StartCoroutine(ShowFailPanelAfterDelay(delay));
        }

        public void ShowWinPanelDelayed(float delay)
        {
            StartCoroutine(ShowWinPanelAfterDelay(delay));
        }
        
        private IEnumerator ShowFailPanelAfterDelay(float delay)
        {
            Debug.Log("Fail Panel Delayed");
            yield return new WaitForSeconds(delay);
            failPanel.SetActive(true);
        }

        private IEnumerator ShowWinPanelAfterDelay(float delay)
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
        
        public void OnFailLevelButtonClicked()
        {
            failPanel.SetActive(false);
            (_gameManager as GameManager)?.RestartFromLastPlatform();
        }
    }
}