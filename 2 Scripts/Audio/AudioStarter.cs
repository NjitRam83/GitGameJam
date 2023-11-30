using BackpackSurvivors.MainGame;
using BackpackSurvivors.Shared;
using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Audio
{
    public class AudioStarter : MonoBehaviour
    {
        [SerializeField] AudioClip _audioClipToPlay;
        [SerializeField] Enums.AudioType _audioType;
        [SerializeField] float _baseVolume;
        [SerializeField] bool _loop = true;

        private void Start()
        {
            StartCoroutine(PlayClip());
        }

        private IEnumerator PlayClip()
        {
            yield return new WaitForSeconds(0.2f);
            switch (_audioType)
            {
                case Enums.AudioType.Music: 
                    {
                        if (_audioClipToPlay != null)
                        {
                            AudioController.instance.PlayMusicClip(_audioClipToPlay, _baseVolume, _loop); break;
                        }
                    }
                    break;
                case Enums.AudioType.Ambiance: { AudioController.instance.PlayAmbianceClip(_audioClipToPlay, _baseVolume, _loop); break; }
            }


            
        }
    }
}
