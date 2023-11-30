using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.MainGame
{
    public class StatsController : MonoBehaviour
    {
        [SerializeField] public SerializableDictionaryBase<StatType, float> _minimumStatValues;
        [SerializeField] public SerializableDictionaryBase<StatType, float> _maximumStatValues;

        public static StatsController instance;

        private Dictionary<StatType, float> _playerStatTypePassiveValues = new Dictionary<StatType, float>();
        private Dictionary<StatType, float> _enemyStatTypePassiveValues = new Dictionary<StatType, float>();
        private Dictionary<StatType, float> _backpackStatTypePassiveValues = new Dictionary<StatType, float>();

        public void ClearStats()
        { 
            _playerStatTypePassiveValues.Clear();
            _enemyStatTypePassiveValues.Clear();
            _backpackStatTypePassiveValues.Clear();

            InitPassivesDictionary(_playerStatTypePassiveValues);
            InitPassivesDictionary(_enemyStatTypePassiveValues);
            InitPassivesDictionary(_backpackStatTypePassiveValues);
        }

        public void ApplyStatsToCharacter(Character character, CharacterType characterType)
        {
            switch (characterType)
            {
                case CharacterType.Player: ApplyPlayerStats(character); return;
                case CharacterType.Enemy: ApplyEnemyStats(character); return;
            }
        }

        private void ApplyPlayerStats(Character character)
        {
            var totalPlayerStats = GetTotalPlayerStats();
            character.SetCharacterStats(totalPlayerStats);
            //character.DebugStats();
        }

        public Dictionary<StatType, float> GetTotalPlayerStats()
        {
            var totalPlayerStats = new Dictionary<StatType, float>();
            InitPassivesDictionary(totalPlayerStats);

            SetTotalPlayerStats(totalPlayerStats);

            foreach (var backpackPassive in _backpackStatTypePassiveValues)
            {
                totalPlayerStats[backpackPassive.Key] += backpackPassive.Value;
            }

            totalPlayerStats = ClampStats(totalPlayerStats);

            return totalPlayerStats;
        }

        public void SetTotalPlayerStats(Dictionary<StatType, float> totalPlayerStats)
        {
            foreach (var playerPassive in _playerStatTypePassiveValues)
            {
                totalPlayerStats[playerPassive.Key] += playerPassive.Value;
            }
        }

        private Dictionary<StatType, float> ClampStats(Dictionary<StatType, float> totalStats)
        {
            var clampedStats = new Dictionary<StatType, float>();

            foreach (var stat in totalStats)
            {
                var minValue = _minimumStatValues[stat.Key];
                var maxValue = _maximumStatValues[stat.Key];
                var currentValue = stat.Value;

                var clampedValue = Mathf.Clamp(currentValue, minValue, maxValue);
                clampedStats.Add(stat.Key, clampedValue);
            }

            return clampedStats;
        }

        private void ApplyEnemyStats(Character character)
        {
            var stats = ClampStats(_enemyStatTypePassiveValues);
            character.SetCharacterStats(stats);
            //character.DebugStats();
        }

        public void AddPassive(GeneratedPassive generatedPassive)
        {
            var targetPassivesDictionary = generatedPassive.StatTarget == StatTarget.Player ? _playerStatTypePassiveValues : _enemyStatTypePassiveValues;
            targetPassivesDictionary[generatedPassive.PassiveType] += generatedPassive.PassiveValue;
        }

        public void ReloadBackpackPassives(Dictionary<StatType, float> backpackPassives)
        {
            _backpackStatTypePassiveValues.Clear();
            _backpackStatTypePassiveValues.AddRange(backpackPassives);
        }

        public float GetStatTypeValue(StatType statType)
        {
            var allStats = GetTotalPlayerStats();
            var statValue = allStats[statType];
            return statValue;
        }

        public Dictionary<StatType, float> GetPlayerPassives()
        {
            var result = new Dictionary<StatType, float>();
            InitPassivesDictionary(result);
            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                result[statType] += _playerStatTypePassiveValues[statType];
                result[statType] += _backpackStatTypePassiveValues[statType];
            }

            return result;
        }

        public Dictionary<StatType, float> GetEnemyPassives()
        {
            var result = new Dictionary<StatType, float>();
            InitPassivesDictionary(result);
            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                result[statType] += _enemyStatTypePassiveValues[statType];
            }

            return result;
        }

        private void InitPassivesDictionary(Dictionary<StatType, float> passivesDictionary)
        {
            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                passivesDictionary.Add(statType, 0f);
            }
        }

        private void Awake()
        {
            SetupSingleton();

            InitPassivesDictionary(_playerStatTypePassiveValues);
            InitPassivesDictionary(_enemyStatTypePassiveValues);
            InitPassivesDictionary(_backpackStatTypePassiveValues);
        }

        private void SetupSingleton()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
