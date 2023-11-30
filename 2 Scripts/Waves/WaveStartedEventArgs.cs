using BackpackSurvivors.Waves.ScriptableObjects;
using System;

namespace BackpackSurvivors.Waves
{
    public class WaveStartedEventArgs : EventArgs
    {
        public WaveStartedEventArgs(WaveSO wave)
        {
            Wave = wave;
        }

        public WaveSO Wave { get; }
    }
}
