using BackpackSurvivors.MainGame;
using BackpackSurvivors.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BackpackSurvivors.UI
{
    public class LevelMenuBarController : MonoBehaviour
    {
        LevelMenuController _levelMenuController;
        [SerializeField] AudioSource _buttonClick;
        private void Start()
        {
            _levelMenuController = FindAnyObjectByType<LevelMenuController>(FindObjectsInactive.Include);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!_levelMenuController.YesNoPopupIsOpen && !SettingsManager.instance.IsOpen)
                {
                    ToggleMainMenu();
                }
                else
                {
                    if (_levelMenuController.YesNoPopupIsOpen)
                    {
                        _levelMenuController.CloseYesNoPopup();
                    }
                    else if (SettingsManager.instance.IsOpen)
                    {
                        SettingsManager.instance.CloseSettingsForm();
                    }
                }
            }
        }

        public void CloseMainMenu()
        {
            _levelMenuController.CloseMainMenu();
        }

        public void OpenMainMenu()
        {
            _levelMenuController.OpenMainMenu();
        }
        public void ToggleMainMenu()
        {
            //_buttonClick.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(_buttonClick);
            if (!_levelMenuController.IsOpen())
            {
                OpenMainMenu();
            }
            else
            {
                CloseMainMenu();
            }
        }

        public void OpenSettings()
        {
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
        public void OpenMountain()
        {

        }
    }
}