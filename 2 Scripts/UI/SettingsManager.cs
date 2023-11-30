using BackpackSurvivors.MainGame;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace BackpackSurvivors.UI
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager instance;

        [SerializeField] GameObject _visualContainer;
        [SerializeField] Animator _animator;
        public bool IsOpen;

        [SerializeField] private float _musicVolumePercentage;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private TextMeshProUGUI _musicVolumeValueText;

        [SerializeField] private float _ambianceVolumePercentage;
        [SerializeField] private Slider _ambianceVolumeSlider;
        [SerializeField] private TextMeshProUGUI _ambianceVolumeValueText;

        [SerializeField] private float _sfxVolumePercentage;
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private TextMeshProUGUI _sfxVolumeValueText;

        private void SetupSingleton()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Awake()
        {
            SetupSingleton();
        }


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowSettingsForm()
        {
            var levelMenuController = FindAnyObjectByType<LevelMenuController>(FindObjectsInactive.Include);
            if (levelMenuController != null && !levelMenuController.IsOpen())
            {
                Time.timeScale = 0f;
            }

            IsOpen = true;
            _visualContainer.SetActive(true);
            _animator.SetBool("Showing", true);
        }

        public void CloseSettingsForm()
        {
            Tooltip.TooltipSystem.Hide();
            var levelMenuController = FindAnyObjectByType<LevelMenuController>(FindObjectsInactive.Include);
            if (levelMenuController != null && !levelMenuController.IsOpen())
            {
                Time.timeScale = 1f;
            }

            IsOpen = false;
            _animator.SetBool("Showing", false);
            StartCoroutine(DoHide());
        }
        private IEnumerator DoHide()
        {
            yield return new WaitForSeconds(0.3f);
            if (!IsOpen)
            {
                _visualContainer.SetActive(false);
            }
        }

        #region UI Events
        public void OnMusicVolumeChanged()
        {
            _musicVolumePercentage = _musicVolumeSlider.value;
            _musicVolumeValueText.SetText(string.Format("{0}%", String.Format("{0:N0}", _musicVolumeSlider.value * 100f)));
            AudioController.instance.SetVolume(Shared.Enums.AudioType.Music, _musicVolumePercentage);
        }

        public void OnAmbianceVolumeChanged()
        {
            _ambianceVolumePercentage = _ambianceVolumeSlider.value;
            _ambianceVolumeValueText.SetText(string.Format("{0}%", String.Format("{0:N0}", _ambianceVolumeSlider.value * 100f)));
            AudioController.instance.SetVolume(Shared.Enums.AudioType.Ambiance, _ambianceVolumePercentage);
        }

        public void OnSFXVolumeChanged()
        {
            _sfxVolumePercentage = _sfxVolumeSlider.value;
            _sfxVolumeValueText.SetText(string.Format("{0}%", String.Format("{0:N0}", _sfxVolumeSlider.value * 100f)));
            AudioController.instance.SetVolume(Shared.Enums.AudioType.SFX, _sfxVolumePercentage);
        }
        #endregion
    }
}