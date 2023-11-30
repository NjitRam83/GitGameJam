using BackpackSurvivors.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.MainGame
{
    public class AudioController : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] AudioSource _musicSource1;
        [SerializeField] AudioSource _musicSource2;
        [SerializeField] float _fadeOutTime;
        [SerializeField] float _fadeInTime;
        private float _baseVolumeMusicSource1;
        private float _baseVolumeMusicSource2;

        [Header("Ambiance")]
        [SerializeField] AudioSource _ambianceSource;
        private float _baseVolumeAmbianceSource;

        [Header("SFX")]
        [SerializeField] List<AudioSource> _sfxSources;

        public static AudioController instance;

        private float _musicVolume = 1f;
        private float _ambianceVolume = 1f;
        private float _sfxVolume = 1f;

        public void SetVolume(Enums.AudioType audioType, float volume)
        {
            switch (audioType)
            {
                case Enums.AudioType.Music: { _musicVolume = volume; UpdateMusicVolume(); break; }
                case Enums.AudioType.Ambiance: { _ambianceVolume = volume; UpdateAmbianceVolume(); break; }
                case Enums.AudioType.SFX: { _sfxVolume = volume; UpdateSFXVolume(); break; }
            }
        }

        private void UpdateSFXVolume()
        {
            foreach (var sfxSource in _sfxSources)
            {
                sfxSource.volume = _sfxVolume;
            }
        }

        private void UpdateAmbianceVolume()
        {
            _ambianceSource.volume = _baseVolumeAmbianceSource * _ambianceVolume;
        }

        private void UpdateMusicVolume()
        {
            if (_musicSource1.isPlaying)
            {
                _musicSource1.volume = _baseVolumeMusicSource1 * _musicVolume;
            }

            if (_musicSource2.isPlaying)
            {
                _musicSource2.volume = _baseVolumeMusicSource2 * _musicVolume;
            }
        }

        public void PlayMusicClip(AudioClip audioClip, float baseVolume = 1f, bool loop = true)
        {
            var willBePlayedOnMusicSource1 = _musicSource1.isPlaying ? false : true;
            var audioSourceToFadeOut = _musicSource1.isPlaying ? _musicSource1 : _musicSource2.isPlaying ? _musicSource2 : null;
            var audioSourceToFadeIn = willBePlayedOnMusicSource1 ? _musicSource1 : _musicSource2;

            if (audioSourceToFadeOut != null) { StartCoroutine(FadeOutAudioSource(audioSourceToFadeOut)); }
            audioSourceToFadeIn.clip = audioClip;
            StartCoroutine(FadeInAudioSource(audioSourceToFadeIn, baseVolume, loop : loop)); ;
            SaveBaseVolume(baseVolume, willBePlayedOnMusicSource1);
        }
        public void StopMusicClip()
        {
            _musicSource1.Stop();
        }

        public void PlayAmbianceClip(AudioClip audioClip, float baseVolume = 1f, bool loop = true)
        {
            _ambianceSource.Stop();
            if (audioClip == null) return;
            _ambianceSource.loop = loop;
            _ambianceSource.clip = audioClip;
            _ambianceSource.volume = baseVolume * _ambianceVolume;
            _baseVolumeAmbianceSource = baseVolume;
            _ambianceSource.Play();
        }

        public void PlayAudioSourceAsSfxClip(AudioSource audioSource)
        { 
            if (audioSource == null || audioSource.clip == null) return;
            PlaySFXClip(audioSource.clip, audioSource.volume);
        }

        public void PlaySFXClip(AudioClip audioClip, float baseVolume)
        {
            AudioSource sfxSourceToPlayOn = null;

            foreach (var sfxSource in _sfxSources)
            {
                if (sfxSource.isPlaying) continue;

                sfxSourceToPlayOn = sfxSource;
                break;
            }

            if (sfxSourceToPlayOn == null)
            {
                Debug.Log($"Trying to play SFX clip, but all SFX Sources are playing. {_sfxSources.Count} SFX Sources is not enough !");
                return;
            }

            sfxSourceToPlayOn.clip = audioClip;
            sfxSourceToPlayOn.volume = baseVolume * _sfxVolume;
            sfxSourceToPlayOn.Play();
        }

        private void SaveBaseVolume(float baseVolume, bool willBePlayedOnMusicSource1)
        {
            if (willBePlayedOnMusicSource1)
            {
                _baseVolumeMusicSource1 = baseVolume;
            }
            else
            {
                _baseVolumeMusicSource2 = baseVolume;
            }
        }

        private IEnumerator FadeOutAudioSource(AudioSource audioSource)
        {
            var timeSpentFadingOut = 0f;
            while (timeSpentFadingOut < _fadeOutTime)
            {
                var percentageToLowerVolumeThisFrame = Time.deltaTime / _fadeOutTime;
                audioSource.volume -= percentageToLowerVolumeThisFrame * audioSource.volume;
                timeSpentFadingOut += Time.deltaTime;
                yield return null;
            }

            audioSource.Stop();
        }

        private IEnumerator FadeInAudioSource(AudioSource audioSource, float baseVolume, float fadeInDelay = 0f, bool loop = true)
        {
            yield return new WaitForSeconds(fadeInDelay);

            audioSource.volume = 0f;
            audioSource.loop = loop;
            audioSource.Play();

            var timeSpentFadingIn = 0f;
            while (timeSpentFadingIn < _fadeInTime)
            {
                var percentageToIncreaseVolumeThisFrame = Time.deltaTime / _fadeInTime;
                audioSource.volume += percentageToIncreaseVolumeThisFrame * baseVolume * _musicVolume;
                timeSpentFadingIn += Time.deltaTime;
                yield return null;
            }
        }

        private void SetupSingleton(out bool continueAwake)
        {
            if (instance != null)
            {
                Destroy(gameObject);
                continueAwake = false;
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            continueAwake = true;
        }

        private void Awake()
        {
            SetupSingleton(out bool continueAwake);
            if (continueAwake == false) return;
        }
    }
}