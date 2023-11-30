using BackpackSurvivors.Combat;
using BackpackSurvivors.MainGame;
using BackpackSurvivors.Waves;
using BackpackSurvivors.Waves.ScriptableObjects;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveCompletedPopupController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI PopupText;
    [SerializeField] TextMeshProUGUI CountdownText;
    [SerializeField] Animator WavePopupAnimator;
    [SerializeField] GameObject WavePopupContainer;
    [SerializeField] AudioSource WavePopupShowAudioSource;
    [SerializeField] AudioSource LevelCompletedAudioSource;

    WavesController _wavesController;

    private bool _wentToReward;

    public void Start()
    {
        _wavesController = FindObjectOfType<WavesController>();
        if (_wavesController == null) return;
        _wavesController.OnWaveCompleted += _wavesController_OnWaveCompleted;
    }

    private void _wavesController_OnWaveCompleted(object sender, WaveCompletedEventArgs e)
    {
        if (e.Wave.IsFinalBoss) return;

        if (!e.HasWavesRemaining)
        {
            AudioController.instance.PlayMusicClip(LevelCompletedAudioSource.clip, LevelCompletedAudioSource.volume);
        }

        Show(e.Wave);
    }


    public void Show(WaveSO wave)
    {
        WavePopupContainer.SetActive(true);
        PopupText.SetText(string.Format("Wave {0} complete!", wave.WaveNumber));
        WavePopupContainer.SetActive(true);
        //WavePopupShowAudioSource.Play(0);
        AudioController.instance.PlayAudioSourceAsSfxClip(WavePopupShowAudioSource);
        WavePopupAnimator.SetTrigger("ShowWaveCompletedPopup");

        StartCoroutine(DoCountdown(10));
    }

    IEnumerator DoCountdown(int seconds)
    {
        _wentToReward = false;

        for (int i = seconds; i > 0; i--)
        {
            CountdownText.SetText(string.Format("Continuing in {0}...", i));
            yield return new WaitForSeconds(1f);
        }

        GoReward();
    }

    public void RewardButtonClicked()
    {
        GoReward();
    }

    private void GoReward()
    {
        if (_wentToReward) return;
        _wentToReward = true;

        WavePopupContainer.SetActive(false);
        if (OnWaveCompleteButtonPressed != null)
        {
            OnWaveCompleteButtonPressed(this, new OnWaveCompletedEventArgs());
        }
    }

    public delegate void WaveCompleteButtonPressedHandler(object sender, OnWaveCompletedEventArgs e);
    public event WaveCompleteButtonPressedHandler OnWaveCompleteButtonPressed;
}
