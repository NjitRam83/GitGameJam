using BackpackSurvivors.MainGame;
using BackpackSurvivors.Pickups;
using BackpackSurvivors.Shared;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BackpackSurvivors.Level
{
    public class BossToCreditsTransition : LevelTransitionBase
    {
        [SerializeField] Image _fadeToBlackPanel;
        [SerializeField] TextMeshProUGUI _endgameMessage;
        [SerializeField] float _alphaIncreasePerSecond;
        [SerializeField] Transform _levelTransitionLocation;
        [SerializeField] float _playerMovementSpeed;
        [SerializeField] AudioClip _afterBossFightAudio;
        private Player _player;

        private void Start()
        {
            _player = FindObjectOfType<Player>();
        }

        public override void StartLevelTransition()
        {
            AudioController.instance.PlayMusicClip(_afterBossFightAudio, 0.15f);

            CollectGold();
            StartCoroutine(DoEndgameAnimation());
        }

        private void CollectGold()
        {
            var allCoinPickups = FindObjectsOfType<CoinPickup>();

            foreach (var coin in allCoinPickups)
            {
                coin.SetMoveToPlayer(true, true);
            }
        }


        private IEnumerator KeepPlayerPositionLocked()
        {
            var diff = 0.0001f;
            while (true)
            {
                _player.transform.position = _player.transform.position += new Vector3(diff, diff, diff);
                diff *= -1;

                yield return null;
            }
        }


        private IEnumerator DoEndgameAnimation()
        {
            GameController.instance.FinishLevel(4); // We know this is the boss level so hardcoded 4 is fine for now
            yield return new WaitForSeconds(2f);

            DisablePlayerMovement(_player);

            var isRunningRight = _levelTransitionLocation.position.x > _player.transform.position.x;
            _player.SetPlayerRunningUntilReset(true, isRunningRight);

            var targetPosition = new Vector2(_levelTransitionLocation.position.x, _player.transform.position.y);
            while (_player.transform.position.x != _levelTransitionLocation.position.x)
            {
                var newPosition = Vector2.MoveTowards(_player.transform.position, targetPosition, _playerMovementSpeed * Time.deltaTime);
                _player.transform.position = newPosition;
                yield return null;
            }

            targetPosition = _levelTransitionLocation.position;
            while (_player.transform.position.y != _levelTransitionLocation.position.y)
            {
                var newPosition = Vector2.MoveTowards(_player.transform.position, targetPosition, _playerMovementSpeed * Time.deltaTime);
                _player.transform.position = newPosition;
                yield return null;
            }

            _player.SetPlayerRunningUntilReset(false, isRunningRight);
            StartCoroutine(KeepPlayerPositionLocked());

            yield return new WaitForSeconds(1f);

            StartCoroutine(StartFadeInMessage());

            yield return new WaitForSeconds(5f);


            StartCoroutine(StartFadeOutTransition());
        }

        private IEnumerator StartFadeInMessage()
        {
            _endgameMessage.enabled = true;
            while (_endgameMessage.color.a < 1)
            {
                var newColor = _endgameMessage.color;
                newColor.a += _alphaIncreasePerSecond * Time.deltaTime;
                _endgameMessage.color = newColor;
                yield return null;
            }
        }

        private IEnumerator StartFadeOutTransition()
        {
            _fadeToBlackPanel.enabled = true;
            while (_fadeToBlackPanel.color.a < 1)
            {
                var newColor = _fadeToBlackPanel.color;
                newColor.a += _alphaIncreasePerSecond * Time.deltaTime;
                _fadeToBlackPanel.color = newColor;
                yield return null;
            }

            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene(Constants.Scenes.CreditsScene);
        }
    }
}
