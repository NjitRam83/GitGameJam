using BackpackSurvivors.Backpack;
using BackpackSurvivors.MainGame;
using BackpackSurvivors.Pickups;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Level
{
    public class LevelTransition : LevelTransitionBase
    {
        [SerializeField] float _playerMovementSpeed;
        [SerializeField] Transform _levelTransitionLocation;
        [SerializeField] Image _fadeToBlackPanel;
        [SerializeField] float _alphaIncreasePerSecond;
        [SerializeField] GameObject[] blockadeObjects;

        private Player _player;

        private void Start()
        {
            _player = FindObjectOfType<Player>();
        }

        public override void StartLevelTransition()
        {
            DisablePlayerMovement(_player);
            CollectGold();
            StartCoroutine(StartPlayerMovementToTransitionPoint());
        }



        private void CollectGold()
        {
            var allCoinPickups = FindObjectsOfType<CoinPickup>();

            foreach (var coin in allCoinPickups)
            {
                coin.SetMoveToPlayer(true, true);
            }
        }

        private IEnumerator StartPlayerMovementToTransitionPoint()
        {
            // Disable 'blocking' elements to show the player he can transition higher up the mountain
            foreach (var blockadeObject in blockadeObjects)
            {
                blockadeObject.SetActive(false);
                Destroy(blockadeObject);
            }


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
            StartCoroutine(StartFadeOutTransition());
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
            BackPackController.instance.ChangeState(BackpackState.Reward, false);
        }
    }
}
