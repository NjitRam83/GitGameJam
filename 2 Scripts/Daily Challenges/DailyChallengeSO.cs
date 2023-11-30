using System.Collections.Generic;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.DailyChallenges
{
    [CreateAssetMenu]
    public class DailyChallengeSO : ScriptableObject
    {
        [SerializeField] public SerializableDictionaryBase<StatType, float> PlayerStatValues;
        [SerializeField] public SerializableDictionaryBase<StatType, float> EnemyStatValues;
        [SerializeField] public int DayNumber;
        [SerializeField] public string ChallengeName;
        [SerializeField] public int StartingGold;
        [SerializeField] public List<ItemQuality> UnlockedRarities;
        [SerializeField] public string LeaderboardKey;
    }
}
