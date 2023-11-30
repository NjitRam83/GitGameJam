public class WavePopupMessage
{
    public string Text;

    public float Duration;
    public WavePopupMessageAudio WavePopupMessageAudio;

    public WavePopupMessage(string text, float duration, WavePopupMessageAudio wavePopupMessageAudio)
    {
        this.Text = text;
        this.Duration = duration;
        this.WavePopupMessageAudio = wavePopupMessageAudio;
    }
}
