using BackpackSurvivors.Backpack;
using BackpackSurvivors.DailyChallenges;
using BackpackSurvivors.Enemies;
using BackpackSurvivors.Level.ScriptableObjects;
using BackpackSurvivors.LootLocker;
using BackpackSurvivors.Shared;
using BackpackSurvivors.Waves;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.MainGame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] public LevelSO[] Levels;
        [SerializeField] private bool _useLootLocker;
        public static GameController instance;
        private LevelSO _currentLevel;

        public bool LevelCompleted;
        private List<int> _completedLevelIds = new List<int>();
        public List<int> CompletedLevelIds => _completedLevelIds;
        public int NextLevelIdToLoad;
        private HashSet<ItemQuality> _unlockedRarities = new HashSet<ItemQuality>();
        public HashSet<ItemQuality> UnlockedRarities => _unlockedRarities;
        [SerializeField] public float CurrentEnemyScale;
        [SerializeField] public float CurrentEnemyHealth;
        [SerializeField] public float CurrentEnemyDamage;
        [SerializeField] public float CurrentEnemyReward;
        [SerializeField] float _enemyScaleIncreasePerLevel;
        [SerializeField] float _enemyHealthIncreasePerLevel;
        [SerializeField] float _enemyDamageIncreasePerLevel;
        [SerializeField] float _enemyRewardIncreasePerLevel;

        public GamePlayType GamePlayType = GamePlayType.Normal;
        public DailyChallengeSO CurrentDailyChallenge;

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
        private bool _firstTimeLoaded = false;
        public bool CanDoFirstTimeLoaded = false;

        public void StartNextLevel()
        {
            _currentLevel = Levels.FirstOrDefault(x => x.LevelId == NextLevelIdToLoad);
            if (_currentLevel == null) return;
            //Debug.Log($"Starting level: {_currentLevel.LevelName}");
            SceneManager.LoadScene(_currentLevel.LevelScene, LoadSceneMode.Single);
        }

        public void FinishLevel(int levelId)
        {

            _completedLevelIds.Add(levelId);
            LevelCompleted = true;
            SubmitScoreToLootLocker(levelId);


            CurrentEnemyScale = 1 + (levelId * _enemyScaleIncreasePerLevel);
            CurrentEnemyHealth = 1 + (levelId * _enemyHealthIncreasePerLevel);
            CurrentEnemyDamage = 1 + (levelId * _enemyDamageIncreasePerLevel);
            CurrentEnemyReward = 1 + (levelId * _enemyRewardIncreasePerLevel);
        }

        private void SubmitScoreToLootLocker(int levelId)
        {
            if (_useLootLocker == false) return;
            var leaderboardKey = GetLeaderboardKey();

            LootLockerController.instance.SubmitScore(levelId, leaderboardKey);
        }

        private string GetLeaderboardKey()
        {
            if (GamePlayType == GamePlayType.Normal)
            {
                return Constants.LootLocker.Leaderboards.MaxLevelFinishedKey;
            }

            if (GamePlayType == GamePlayType.DailyChallenge)
            {
                return CurrentDailyChallenge.LeaderboardKey;
            }

            return Constants.LootLocker.Leaderboards.MaxLevelFinishedKey;
        }

        public LevelSO GetLevelById(int levelId)
        {
            var level = Levels.FirstOrDefault(l => l.LevelId == levelId);
            if (level == null) throw new System.Exception($"Level with id {levelId} was not found in GameController");

            return level;
        }

        public void DoFirstTimeLoad()
        {
            CanDoFirstTimeLoaded = true;
        }

        public void ShowShop()
        {
            BackPackController.instance.ChangeState(BackpackState.Shop, false);
        }

        public void AddUnlockedItemQuality(IEnumerable<ItemQuality> itemQualities)
        {
            _unlockedRarities.AddRange(itemQualities);
        }

        private void Awake()
        {
            SetupSingleton(out bool continueAwake);
            if (continueAwake == false) return;
        }

        private void Start()
        {
            StartCoroutine(BindLoadSceneOnChangeState());
            InitUnlockedRarities();

            //AddTestPassives(); //TODO: Remove for build
        }

        private void AddTestPassives()
        {
            var passive = new GeneratedPassive();
            passive.StatTarget = StatTarget.Player;
            passive.PassiveType = StatType.FlatDamage;
            passive.PassiveValue = 1000;

            //StatsController.instance.AddPassive(passive);
        }

        private void InitUnlockedRarities()
        {
            _unlockedRarities.Add(ItemQuality.Common);
            _unlockedRarities.Add(ItemQuality.Uncommon);
        }

        private void _backPackController_OnChangeState(object sender, BackpackChangeStateEventArgs e)
        {
            if (PlayerExitedShop(e) && CurrentlyInMountainScalingScene())
            {
                StartNextLevel();
                return;
            }

            if (PlayerExitedShop(e) && LevelCompleted)
            {
                SceneManager.LoadScene(Constants.Scenes.MountainScalingScene, LoadSceneMode.Single);
                return;
            }
        }

        private bool CurrentlyInMountainScalingScene()
        {
            return SceneManager.GetActiveScene().name == Constants.Scenes.MountainScalingScene;
        }

        private bool PlayerExitedShop(BackpackChangeStateEventArgs e)
        {
            var exitedShop = (e.OldState == BackpackState.Shop) && e.NewState == BackpackState.Hidden;
            var exitShopAfterFirstLoad = (e.OldState == BackpackState.FirstLoad) && e.NewState == BackpackState.Hidden;

            return exitedShop || exitShopAfterFirstLoad;
        }

        private IEnumerator BindLoadSceneOnChangeState()
        {
            while (BackPackController.instance == null)
            {
                yield return null;
            }

            BackPackController.instance.OnChangeState += _backPackController_OnChangeState;
        }

        private void UnbindLoadSceneOnChangeState()
        {
            BackPackController.instance.OnChangeState -= _backPackController_OnChangeState;
        }

        private void Update()
        {
            if (!_firstTimeLoaded && CanDoFirstTimeLoaded)
            {
                BackPackController.instance.Init();
                _firstTimeLoaded = true;
                BackPackController.instance.ChangeState(BackpackState.FirstLoad, false);
            }

            ////DEBUG: TODO: REMOVE
            //if (Input.GetKeyDown(KeyCode.K))
            //{
            //    Debug_KillAllEnemies();
            //}

            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    Debug_FinishLevel();
            //}

            //if (Input.GetKeyDown(KeyCode.J))
            //{
            //    Debug_FinishWave();
            //}

            //if (Input.GetKeyDown(KeyCode.C))
            //{
            //    Debug_Add1000Coins();
            //}
        }

        private void Debug_FinishLevel()
        {
            FindObjectOfType<WavesController>().Debug_FinishLevel();
        }

        private void Debug_FinishWave()
        {
            FindObjectOfType<WavesController>().Debug_FinishWave();
        }

        private void Debug_KillAllEnemies()
        {
            var allEnemies = FindObjectsOfType<Enemy>();
            foreach (var enemy in allEnemies)
            {
                enemy.Debug_Kill();
            }
        }

        private void Debug_Add1000Coins()
        {
            MoneyController.instance.GainCoins(1000);
        }
    }
}