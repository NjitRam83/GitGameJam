using BackpackSurvivors.Items;
using BackpackSurvivors.MainGame;
using BackpackSurvivors.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BackpackSurvivors.DailyChallenges
{
    public class DailyChallengeController : MonoBehaviour
    {
        [SerializeField] List<DailyChallengeSO> _dailyChallenges;
        [SerializeField] TextMeshProUGUI _challengeTitleText;
        [SerializeField] TextMeshProUGUI _modifierContentText;
        [SerializeField] GameObject _grid;
        [SerializeField] GameObject _challengeButtonPrefab;
        [SerializeField] ItemDataBase _itemDataBase;
        [SerializeField] Button _startButton;

        private DailyChallengeSO _selectedDailyChallenge;

        public void BackToMainMenu()
        {
            SceneManager.LoadScene(Constants.Scenes.MainMenuScene, LoadSceneMode.Single);
        }

        private void Start()
        {
            WarnIfNotAllDaynumbersPresent();
            GenerateDailyChallengeButtons();
            RegisterStartButton();
        }

        private void RegisterStartButton()
        {
            _startButton.onClick.AddListener(StartChallenge);
        }

        private void StartChallenge()
        {
            foreach (var stat in _selectedDailyChallenge.PlayerStatValues)
            {
                var passive = new GeneratedPassive();
                passive.StatTarget = Enums.StatTarget.Player;
                passive.PassiveType = stat.Key;
                passive.PassiveValue = stat.Value;
                StatsController.instance.AddPassive(passive);
            }

            foreach (var stat in _selectedDailyChallenge.EnemyStatValues)
            {
                var passive = new GeneratedPassive();
                passive.StatTarget = Enums.StatTarget.Enemy;
                passive.PassiveType = stat.Key;
                passive.PassiveValue = stat.Value;
                StatsController.instance.AddPassive(passive);
            }

            MoneyController.instance.GainCoins(_selectedDailyChallenge.StartingGold - MoneyController.instance.Coins);
            GameController.instance.GamePlayType = Enums.GamePlayType.DailyChallenge;
            GameController.instance.CurrentDailyChallenge = _selectedDailyChallenge;
            GameController.instance.UnlockedRarities.AddRange(_selectedDailyChallenge.UnlockedRarities);

            SceneManager.LoadScene(Constants.Scenes.MountainScalingScene, LoadSceneMode.Single);
        }

        private void GenerateDailyChallengeButtons()
        {
            for (int i = 1; i <= 31; i++)
            {
                var buttonGameObject = Instantiate(_challengeButtonPrefab, _grid.transform, false);
                var dailyChallengeButton = buttonGameObject.GetComponent<DailyChallengeButton>();
                dailyChallengeButton.SetDailyChallengeSO(_dailyChallenges.First(c => c.DayNumber == i));

                if (i > DateTime.Today.Day)
                {
                    buttonGameObject.GetComponent<Button>().interactable = false;
                }
            }
        }

        public DailyChallengeSO GetChallengeForToday()
        {
            var dayNumber = DateTime.Today.Day;
            var challengeForToday = _dailyChallenges.FirstOrDefault(dc => dc.DayNumber == dayNumber);

            return challengeForToday;
        }

        private void WarnIfNotAllDaynumbersPresent()
        {
            var distinctDayNumbers = _dailyChallenges.Select(dc => dc.DayNumber).Distinct();
            if (distinctDayNumbers.Min() != 1) { Debug.LogWarning("Lowest available daynumber in daily challenges should be 1"); }
            if (distinctDayNumbers.Max() != 31) { Debug.LogWarning("Highest available daynumber in daily challenges should be 31"); }
            if (distinctDayNumbers.Count() != 31) { Debug.LogWarning("Number of available daily challenges should be 31"); }
        }

        internal void ChallengeSelected(DailyChallengeSO dailyChallengeSO)
        {
            _selectedDailyChallenge = dailyChallengeSO;
            _challengeTitleText.text = _selectedDailyChallenge.ChallengeName;
            _modifierContentText.text = GetModifierText(_selectedDailyChallenge);
            _startButton.interactable = true;
        }

        private string GetModifierText(DailyChallengeSO dailyChallengeSO)
        {
            var sb = new StringBuilder();

            if (dailyChallengeSO.PlayerStatValues.Any()) { sb.AppendLine($"<color=#00FF21>Player:</color> "); } 
            foreach (var stat in dailyChallengeSO.PlayerStatValues)
            {
                var statTitle = _itemDataBase.GetCleanString(stat.Key);
                var statValue = _itemDataBase.GetCleanValue(stat.Value, stat.Key);
                var modifierText = $"{statTitle}: {statValue}";
                sb.AppendLine(modifierText);
            }

            if (dailyChallengeSO.EnemyStatValues.Any()) { sb.AppendLine($"<color=#CF0000>Enemies: </color>"); }
            foreach (var stat in dailyChallengeSO.EnemyStatValues)
            {
                var statTitle = _itemDataBase.GetCleanString(stat.Key);
                var statValue = _itemDataBase.GetCleanValue(stat.Value, stat.Key);
                var modifierText = $"{statTitle}: {statValue}";
                sb.AppendLine(modifierText);
            }

            if (dailyChallengeSO.StartingGold > 0)
            {
                sb.AppendLine($"<color=#FBBE4B>Gold: {dailyChallengeSO.StartingGold}</color>");
            }

            foreach (var rarity in dailyChallengeSO.UnlockedRarities)
            {
                var colorCode = _itemDataBase.GetColorHexcodeForRarity(rarity);
                sb.AppendLine($"<color=#{colorCode}>{rarity}</color> unlocked");
            }

            return sb.ToString();
        }
    }
}
