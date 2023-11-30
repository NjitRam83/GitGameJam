using BackpackSurvivors.MainGame;
using System.Collections;
using UnityEngine;

public class RandomAudioPicker : MonoBehaviour
{

    AudioSource _sourceToPlayFrom;

    [SerializeField] AudioClip[] _clipsToChooseFrom;
    [SerializeField] int _randomMax;
    [SerializeField] bool _enabled;


    // Start is called before the first frame update
    void Start()
    {
        _sourceToPlayFrom = GetComponent<AudioSource>();
        StartCoroutine(StartAttemptingPlays());
    }

    IEnumerator StartAttemptingPlays()
    {
        while (_enabled)
        {
            int random = Random.Range(0, _randomMax);
            if (random == 0)
            {
                int randomClip = Random.Range(0, _clipsToChooseFrom.Length);
                PlayAudio(_clipsToChooseFrom[randomClip]);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void PlayAudio(AudioClip clip)
    {
        _sourceToPlayFrom.clip = clip;
        //_sourceToPlayFrom.Play(0);
        AudioController.instance.PlayAudioSourceAsSfxClip(_sourceToPlayFrom);
    }
}
