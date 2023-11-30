using BackpackSurvivors.MainGame;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI
{
    public class PlayerDeathUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _countdownText;
        [SerializeField] AudioSource _diedAudio;

        public void ShowPlayerDeathUI()
        {
            gameObject.SetActive(true);
            AudioController.instance.StopMusicClip();
            AudioController.instance.PlayAudioSourceAsSfxClip(_diedAudio);
        }

        public void UpdateCountdownText(string text)
        {
            _countdownText.text = text;
        }
    }
}
