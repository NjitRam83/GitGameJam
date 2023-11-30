using BackpackSurvivors.Waves.ScriptableObjects;
using System;

namespace BackpackSurvivors.Waves
{
    public class WaveCompletedEventArgs : EventArgs
    {
        public WaveCompletedEventArgs(WaveSO wave, bool hasWavesRemaining)
        {
            Wave = wave;
            HasWavesRemaining = hasWavesRemaining;
        }

        public WaveSO Wave { get; }
        public bool HasWavesRemaining { get; }
    }
}
