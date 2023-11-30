using BackpackSurvivors.LootLocker;
using BackpackSurvivors.MainGame;
using BackpackSurvivors.Shared;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BackpackSurvivors.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] AudioSource _buttonClick;
        [SerializeField] GameObject _playMenuContainer;
        [SerializeField] float _xSlidePositionChange;
        [SerializeField] float _slideDuration;
        [SerializeField] Button _buttonNewGame;

        private LootLockerSessionStarter _lootLockerSessionStarted;
        private bool _playMenuSlidOut;

        private void Start()
        {
            _lootLockerSessionStarted = FindObjectOfType<LootLockerSessionStarter>();
        }

        private void Update()
        {
            // This messes with player input in the text field
            // Either check whether the textfield has focus and only execute navigation if it does not
            // Or have a leaderboard overlay that can be activated on this screen, show the player name input in there and disable input navigation if showing the leaderboard
            // If we do not implement leaderboards we do not need the input field, so in that case the input navigation can be enabled again without issues

            //if (Input.GetKeyDown(KeyCode.N))
            //{
            //    NavigateToMountainScene();
            //}
            //else if (Input.GetKeyDown(KeyCode.H))
            //{
            //    NavigateToHowToPlayScene();
            //}
            //else if (Input.GetKeyDown(KeyCode.S))
            //{
            //    NavigateToSettingsScene();
            //}
            //else if (Input.GetKeyDown(KeyCode.C))
            //{
            //    NavigateToCreditsScene();
            //}
        }

        public void NavigateToHowToPlayScene()
        {
            StatsController.instance.ClearStats();
            NavigateToScene(Constants.Scenes.ExplainationScene);
        }
        public void NavigateToDailyChallenges()
        {
            StatsController.instance.ClearStats();
            NavigateToScene(Constants.Scenes.DailyChallengesScene);
        }
        public void NavigateToMountainScene()
        {
            StatsController.instance.ClearStats();
            NavigateToScene(Constants.Scenes.MountainScalingScene);
        }
        public void NavigateToSettingsScene()
        {
            SettingsManager.instance.ShowSettingsForm();
        }
        public void NavigateToCreditsScene()
        {
            NavigateToScene(Constants.Scenes.CreditsScene);
        }

        public void ExpandPlayMenu()
        {
            if (_playMenuSlidOut) return;
            _buttonNewGame.interactable = false;
            StartCoroutine(SlideoutPlayMenu());
        }

        private IEnumerator SlideoutPlayMenu()
        {
            _playMenuSlidOut = true;

            var resolutionModifier = 1;// (Screen.width / Screen.height) / 1.6f;
            Debug.Log(resolutionModifier);
            var targetXPositionChange = _xSlidePositionChange * resolutionModifier; //152;// (_expandedMenuTargetX - _playMenuContainer.transform.position.x);// / 60;// / 60; //Because the canvas is set to scale of 1/60 (why?)
            var timeSpentSliding = 0f;

            while (timeSpentSliding < _slideDuration)
            {
                timeSpentSliding += Time.deltaTime;
                var percentageSlidThisFrame = Time.deltaTime / _slideDuration;
                var xToAdd = percentageSlidThisFrame * targetXPositionChange;
                var slideVector = new Vector3(xToAdd, 0, 0);
                _playMenuContainer.transform.position += slideVector / 60; //Because the canvas is set to scale of 1/60 (why?)

                yield return null;
            }
        }

        private void NavigateToScene(string sceneName)
        {
            GameController.instance.GamePlayType = Enums.GamePlayType.Normal;
            _lootLockerSessionStarted.SetNameInLootLocker();
            //_buttonClick.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(_buttonClick);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}