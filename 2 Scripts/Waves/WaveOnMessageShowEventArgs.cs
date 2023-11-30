using System;
using System.Collections.Generic;

namespace BackpackSurvivors.Waves
{
    public class WaveOnMessageShowEventArgs : EventArgs
    {


        public WaveOnMessageShowEventArgs(List<WavePopupMessage> messages, bool audioOnShow)
        {
            Messages = messages;
            AudioOnShow = audioOnShow;
        }

        public WaveOnMessageShowEventArgs(string text, bool audioOnShow, float secondsToShow = 4.5f)
        {
            Text = text;
            AudioOnShow = audioOnShow;
            SecondsToShow = secondsToShow;
        }

        public List<WavePopupMessage> Messages { get; }
        public bool AudioOnShow { get; }
        public string Text { get; }
        public float SecondsToShow { get; }
    }
}
