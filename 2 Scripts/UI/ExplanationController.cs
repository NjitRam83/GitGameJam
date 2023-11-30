using BackpackSurvivors.MainGame;
using BackpackSurvivors.Shared;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BackpackSurvivors.UI
{
    public class ExplanationController : MonoBehaviour
    {
        [SerializeField] GameObject IntroductionStateObject;
        [SerializeField] GameObject CombinationStateObject;
        [SerializeField] GameObject BackpackStateObject;
        [SerializeField] GameObject ExtensionsStateObject;
        [SerializeField] GameObject ItemsStateObject;
        [SerializeField] GameObject WavesStateObject;
        [SerializeField] GameObject SurviveStateObject;
        [SerializeField] TextMeshProUGUI NextButtonText;
        [SerializeField] GameObject SkipButton;
        [SerializeField] GameObject PrevButton;
        [SerializeField] AudioSource _buttonClick;

        ExplanationState _currentState = ExplanationState.IntroductionState;
        Dictionary<ExplanationState, GameObject> _currentStateAndPanels = new Dictionary<ExplanationState, GameObject>();

        private void Start()
        {
            _currentStateAndPanels = new Dictionary<ExplanationState, GameObject>();
            _currentStateAndPanels.Add(ExplanationState.IntroductionState, IntroductionStateObject);
            _currentStateAndPanels.Add(ExplanationState.CombinationState, CombinationStateObject);
            _currentStateAndPanels.Add(ExplanationState.BackpackState, BackpackStateObject);
            _currentStateAndPanels.Add(ExplanationState.ExtensionsState, ExtensionsStateObject);
            _currentStateAndPanels.Add(ExplanationState.ItemsState, ItemsStateObject);
            _currentStateAndPanels.Add(ExplanationState.WavesState, WavesStateObject);
            _currentStateAndPanels.Add(ExplanationState.SurviveState, SurviveStateObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                GoPrevState();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                GoNextState();
            }
        }


        public void GoNextState()
        {
            //_buttonClick.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(_buttonClick);
            _currentStateAndPanels[_currentState].SetActive(false);
            int currentState = (int)_currentState;
            if (currentState < _currentStateAndPanels.Count - 1)
            {
                currentState++;
                PrevButton.SetActive(true);
                _currentState = (ExplanationState)currentState;
                _currentStateAndPanels[_currentState].SetActive(true);
                if (_currentState == ExplanationState.SurviveState)
                {
                    NextButtonText.SetText("START");
                    SkipButton.SetActive(false);
                }
            }
            else
            {
                GoNext();
            }
        }
        public void GoPrevState()
        {
            //_buttonClick.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(_buttonClick);
            int currentState = (int)_currentState;
            if (currentState > 0)
            {
                _currentStateAndPanels[_currentState].SetActive(false);
                currentState--;
                _currentState = (ExplanationState)currentState;
                _currentStateAndPanels[_currentState].SetActive(true);

                PrevButton.SetActive(_currentState != ExplanationState.IntroductionState);
                NextButtonText.SetText("NEXT");
                SkipButton.SetActive(true);
            }
            else
            {
                PrevButton.SetActive(false);
            }
        }

        public void GoNext()
        {
            SceneManager.LoadScene(Constants.Scenes.StoryScene, LoadSceneMode.Single);
        }
    }

    public enum ExplanationState
    {
        IntroductionState,
        CombinationState,
        BackpackState,
        ExtensionsState,
        ItemsState,
        WavesState,
        SurviveState,
    }
}