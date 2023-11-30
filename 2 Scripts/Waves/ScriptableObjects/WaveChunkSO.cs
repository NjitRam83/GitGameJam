using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Waves.ScriptableObjects
{
    [CreateAssetMenu]
    public class WaveChunkSO : ScriptableObject
    {
        [SerializeField] public GameObject EnemyPrefab;
        [SerializeField] public int NumberOfEnemiesToSpawn;
        [SerializeField] public float StartSpawningAfterDelay;
        [SerializeField] public float SpawnDelayBetweenEnemies;
        [SerializeField] public WavePositionSpawnType WavePositionSpawnType;

        public bool Spawned;
    }
}