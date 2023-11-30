using BackpackSurvivors.Waves.ScriptableObjects;
using System.Collections.Generic;
using Tymski;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Level.ScriptableObjects
{
    [CreateAssetMenu]
    public class LevelSO : ScriptableObject
    {
        [SerializeField] public int LevelId;
        [SerializeField] public string LevelName;
        [SerializeField] public string Description;
        [SerializeField] public List<WaveSO> Waves;
        [SerializeField] public SceneReference LevelScene;

        [SerializeField] public LevelRank LevelRank;

        [SerializeField] public int BestScore;
        [SerializeField] public int BestTime;
        [SerializeField] public int BestCoins;

        [SerializeField] public List<ItemQuality> ItemQualitiesToUnlock;

    }

    public enum LevelRank
    {
        None = 0,
        Bronze = 1,
        Silver = 2,
        Gold = 3
    }
}
