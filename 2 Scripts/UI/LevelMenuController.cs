using BackpackSurvivors.MainGame;
using BackpackSurvivors.Shared;
using BackpackSurvivors.UI.Tooltip;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BackpackSurvivors.UI
{
    public class LevelMenuController : MonoBehaviour
    {
        [SerializeField] YesNoPopupController _yesNoPopupController;
        [SerializeField] AudioSource _buttonClick;
        [SerializeField] Animator _animator;
        private bool _isOpen = false;


        public bool YesNoPopupIsOpen => _yesNoPopupController.IsOpen;

        public void CloseMainMenu()
        {
            PlayButtonClick();
            _animator.SetBool("Showing", false);
            
            Time.timeScale = 1f;
            StartCoroutine(DoHide());
        }
        private IEnumerator DoHide()
        {
            yield return new WaitForSeconds(1f);
            TooltipSystem.Hide();
            gameObject.SetActive(false);
            _isOpen = false;
        }

        public void OpenMainMenu()
        {
            PlayButtonClick();

            Time.timeScale = 0f;
            gameObject.SetActive(true);
            _animator.SetBool("Showing", true);
            _isOpen = true;
        }

        public void OpenSettings()
        {
            PlayButtonClick();
            if (SettingsManager.instance != null)
            {
                if (SettingsManager.instance.IsOpen)
                {
                    SettingsManager.instance.CloseSettingsForm();
                }
                else
                {
                    SettingsManager.instance.ShowSettingsForm();
                }
            }
        }
        public void GoMountain()
        {
            PlayButtonClick();
            _yesNoPopupController.ClearEvents();

            _yesNoPopupController.OnYesPressed += GoMountain_OnYesPressed;
            _yesNoPopupController.OnNoPressed += GoMountain_OnNoPressed;

            _yesNoPopupController.Show("This will exit the level", "ARE YOU SURE?");
        }

        private void GoMountain_OnYesPressed(object sender, YesPressedEventArgs e)
        {
            PlayButtonClick();
            CloseMainMenu();
            SceneManager.LoadScene(Constants.Scenes.MountainScalingScene);
        }
        private void GoMountain_OnNoPressed(object sender, NoPressedEventArgs e)
        {
            PlayButtonClick();
            _yesNoPopupController.Hide();
        }

        private void GoMain_OnYesPressed(object sender, YesPressedEventArgs e)
        {
            PlayButtonClick();
            CloseMainMenu();
            SceneManager.LoadScene(Constants.Scenes.MainMenuScene);
        }
        private void GoMain_OnNoPressed(object sender, NoPressedEventArgs e)
        {
            PlayButtonClick();
            _yesNoPopupController.Hide();
        }

        public void GoMain()
        {
            PlayButtonClick();
            _yesNoPopupController.ClearEvents();

            _yesNoPopupController.OnYesPressed += GoMain_OnYesPressed;
            _yesNoPopupController.OnNoPressed += GoMain_OnNoPressed;

            _yesNoPopupController.Show("This will lose all progress", "ARE YOU SURE?");
        }

        private void PlayButtonClick()
        {
            AudioController.instance.PlaySFXClip(_buttonClick.clip, _buttonClick.volume);
        }

        internal bool IsOpen()
        {
            return _isOpen;
        }

        internal void CloseYesNoPopup()
        {
            _yesNoPopupController.Hide();
        }
    }
}