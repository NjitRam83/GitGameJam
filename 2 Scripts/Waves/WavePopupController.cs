using BackpackSurvivors.MainGame;
using BackpackSurvivors.Waves;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class WavePopupController : MonoBehaviour
{
    [SerializeField] Image IconImage;
    [SerializeField] TextMeshProUGUI PopupText;
    [SerializeField] Animator WavePopupAnimator;
    [SerializeField] GameObject WavePopupContainer;
    [SerializeField] AudioSource WavePopupShowAudioSource;
    [SerializeField] AudioSource WavePopupChingAudioSource;
    [SerializeField] AudioSource WavePopupCountdownAudioSource;
    [SerializeField] AudioSource WavePopupGoAudioSource;

    float _audioDelayDuration = 0.5f;
    float _hidePopupAnimationDuration = 1.5f;
    float _showPopupAnimationDuration = 0.5f;
    WavesController _wavesController;
    bool _currentlyShowing;

    public void Start()
    {
        _wavesController = FindObjectOfType<WavesController>();
        if (_wavesController == null) return;
        _wavesController.OnWaveOnMessageShow += _wavesController_OnWaveOnMessageShow;
    }

    private void _wavesController_OnWaveOnMessageShow(object sender, WaveOnMessageShowEventArgs e)
    {
        if (e.Messages != null)
        {
            Show(e.Messages, e.AudioOnShow);
        }
        else
        {
            Show(e.Text, e.AudioOnShow, e.SecondsToShow);
        }
    }

    public void Show(List<WavePopupMessage> messages, bool audioOnShow)
    {
        if (_currentlyShowing) return;

        _currentlyShowing = true;
        PopupText.SetText("");
        WavePopupContainer.SetActive(true);
        WavePopupAnimator.SetTrigger("ShowWavePopup");
        StartCoroutine(ShowPopup(messages, audioOnShow));
    }

    IEnumerator ShowPopup(List<WavePopupMessage> messages, bool audioOnShow)
    {
        yield return new WaitForSeconds(_audioDelayDuration);
        if (audioOnShow)
        {
            //WavePopupShowAudioSource.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(WavePopupShowAudioSource);
        }
        yield return new WaitForSeconds(_showPopupAnimationDuration);
        foreach (WavePopupMessage message in messages)
        {
            PopupText.SetText(message.Text);

            AudioSource audioToPlay = GetWavePopupMessageAudio(message.WavePopupMessageAudio);
            if (audioToPlay != null)
            {
                //audioToPlay.Play(0);
                AudioController.instance.PlayAudioSourceAsSfxClip(audioToPlay);
            }

            yield return new WaitForSeconds(message.Duration);
        }

        WavePopupAnimator.SetTrigger("HideWavePopup");
        yield return new WaitForSeconds(_hidePopupAnimationDuration);
        _currentlyShowing = false;
        WavePopupContainer.SetActive(false);
    }



    public void Show(string text, bool audioOnShow, float secondsToShow = 4.5f)
    {
        if (_currentlyShowing) return;
        _currentlyShowing = true;

        WavePopupContainer.SetActive(true);
        PopupText.SetText(text);
        WavePopupAnimator.SetTrigger("ShowWavePopup");
        StartCoroutine(ShowPopup(audioOnShow, secondsToShow));
    }
    IEnumerator ShowPopup(bool audioOnShow, float secondsToShow = 4.5f)
    {
        yield return new WaitForSeconds(_audioDelayDuration);
        if (audioOnShow)
        {
            //WavePopupShowAudioSource.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(WavePopupShowAudioSource);
        }
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(secondsToShow - _hidePopupAnimationDuration - _audioDelayDuration);
        WavePopupAnimator.SetTrigger("HideWavePopup");
        yield return new WaitForSeconds(_hidePopupAnimationDuration);
        _currentlyShowing = false;
        WavePopupContainer.SetActive(false);
    }

    private AudioSource GetWavePopupMessageAudio(WavePopupMessageAudio wavePopupMessageAudio)
    {
        switch (wavePopupMessageAudio)
        {
            case WavePopupMessageAudio.None:
                return null;
            case WavePopupMessageAudio.Ching:
                return WavePopupChingAudioSource;
            case WavePopupMessageAudio.CountdownAudio:
                return WavePopupCountdownAudioSource;
            case WavePopupMessageAudio.GoAudio:
                return WavePopupGoAudioSource;
            default:
                return null;
        }
    }
}
