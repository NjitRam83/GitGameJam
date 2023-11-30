using BackpackSurvivors.Backpack;
using BackpackSurvivors.Combat;
using BackpackSurvivors.Enemies;
using BackpackSurvivors.Level.ScriptableObjects;
using BackpackSurvivors.MainGame;
using BackpackSurvivors.Waves;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Level
{
    public class LevelController : MonoBehaviour
    {

        [SerializeField] int _levelId;
        private LevelSO _levelSO;

        private WavesController _waveController;
        private CombatController _combatController;
        private WaveCompletedPopupController _waveCompletedPopupController;
        //private ShopVendor _shopVendor;

        private void Start()
        {
            RegisterControllers();
            _levelSO = GameController.instance.GetLevelById(_levelId);
            BackPackController.instance.OnChangeState += BackpackController_OnChangeState;
            GameController.instance.AddUnlockedItemQuality(_levelSO.ItemQualitiesToUnlock);
            //_shopVendor = FindObjectOfType<ShopVendor>();
            //_shopVendor.OnShopVendorReached += _shopVendor_OnShopVendorReached;

            StartLevel();
        }
        public void StartLevel()
        {
            _combatController.ReloadStats();
            _combatController.InitLevel();
            _waveController.InitLevel(_levelSO);

            _waveCompletedPopupController = FindObjectOfType<WaveCompletedPopupController>();
            _waveCompletedPopupController.OnWaveCompleteButtonPressed += _waveCompletedPopupController_OnWaveCompleteButtonPressed;

            GameController.instance.LevelCompleted = false;

            var countdownDelay = _waveController.GetCountdownLenght();
            StartCoroutine(DoStartNextWave(countdownDelay));
        }

        public void FinishLevel()
        {
            GameController.instance.FinishLevel(_levelSO.LevelId);
        }

        //private void _shopVendor_OnShopVendorReached(object sender, ShopVendorReachedEventArgs e)
        //{
        //    // pause game?
        //    Time.timeScale = 0f;
        //    BackPackController.instance.ChangeState(BackpackState.Shop, true);
        //}

        private void _waveCompletedPopupController_OnWaveCompleteButtonPressed(object sender, OnWaveCompletedEventArgs e)
        {
            WaveCompletePlayerReadyButtonPressed();
        }

        private IEnumerator DoStartNextWave(float delay)
        {
            if (_waveController.HasWavesRemaining())
            {
                _waveController.ShowWaveStartingMessage();

                yield return new WaitForSeconds(delay);
                Time.timeScale = 1;

                _combatController.ReloadStats();
                _waveController.StartNextWave();
                _combatController.SetCanAct(true);
                BackPackController.instance.ResetRerollCost();
            }
        }

        private void RegisterControllers()
        {
            _waveController = FindObjectOfType<WavesController>();
            _waveController.OnWaveCompleted += WaveController_OnWaveCompleted;

            _combatController = FindObjectOfType<CombatController>();

        }

        private void BackpackController_OnChangeState(object sender, BackpackChangeStateEventArgs e)
        {
            if (e.NewState == BackpackState.Hidden && e.FromVendor)
            {
                Time.timeScale = 1f;
            }

            if (e.NewState == BackpackState.Hidden && !e.FromVendor)
            {
                StartCoroutine(DoStartNextWave(4f));
            }
        }

        private void WaveCompletePlayerReadyButtonPressed()
        {
            if (_waveController.HasWavesRemaining() == false)
            {
                FinishLevel();
                BackPackController.instance.OnChangeState -= BackpackController_OnChangeState;

                var levelTransition = FindObjectOfType<LevelTransitionBase>();
                levelTransition.StartLevelTransition();
                return;
            }
            BackPackController.instance.ChangeState(BackpackState.Reward, false);
        }

        private void WaveController_OnWaveCompleted(object sender, WaveCompletedEventArgs e)
        {
            _combatController.SetCanAct(false);

            //custom to end game:
            if (e.Wave.IsFinalBoss)
            {
                var levelTransition = FindObjectOfType<LevelTransitionBase>();
                levelTransition.StartLevelTransition();
            }
        }

        public void KillEnemy()
        {
            //For testing purposes

            var enemy = FindObjectOfType<Enemy>();
            if (enemy == null) return;

            var enemyHealth = enemy.GetComponent<Health>();
            enemyHealth.Kill();
        }

        private void OnDestroy()
        {
            _waveCompletedPopupController = FindObjectOfType<WaveCompletedPopupController>();
            if (_waveCompletedPopupController != null)
            {
                _waveCompletedPopupController.OnWaveCompleteButtonPressed -= _waveCompletedPopupController_OnWaveCompleteButtonPressed;
            }

            _waveController = FindObjectOfType<WavesController>();
            if (_waveController != null)
            {
                _waveController.OnWaveCompleted -= WaveController_OnWaveCompleted;
            }

            BackPackController.instance.OnChangeState -= BackpackController_OnChangeState;
        }
    }
}
