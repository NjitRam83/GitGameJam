using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.Waves.ScriptableObjects
{
    [CreateAssetMenu]
    public class WaveSO : ScriptableObject
    {
        [SerializeField] public int WaveNumber;
        [SerializeField] public bool IsFinalBoss;
        [SerializeField] public List<WaveChunkSO> WaveChunks;
    }
}