using BackpackSurvivors.Enemies;
using BackpackSurvivors.Level.ScriptableObjects;
using BackpackSurvivors.MainGame;
using BackpackSurvivors.Shared;
using BackpackSurvivors.Waves.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BackpackSurvivors.Waves
{
    public class WavesController : MonoBehaviour
    {
        [SerializeField] List<WaveSO> _waveSOs;
        [SerializeField] GameObject _enemiesParentGameObject;
        [SerializeField] float _minOffsetToPlayer;
        [SerializeField] GameObject _spawnLocationsParentGameObject;
        private List<WaveChunkSO> _waveChunks = new List<WaveChunkSO>();
        private float _timePassedInWave;
        private bool _canAct;
        private int _currentWaveNumber = 0;
        private WaveSO _currentWave;
        private EnemyController _enemyController;
        private Player _player;
        private bool _canSpawn = true;

        public void InitLevel(LevelSO levelSO)
        {
            _waveSOs.Clear();
            _waveSOs.AddRange(levelSO.Waves);

            StartCoroutine(ShowMessagesDelayed());
        }

        private IEnumerator ShowMessagesDelayed()
        {
            var maxSecondsToWait = 2f;
            var timeWaited = 0f;
            while (timeWaited < maxSecondsToWait && OnWaveOnMessageShow == null)
            {
                yield return null;
                timeWaited += Time.deltaTime;
            }

            if (OnWaveOnMessageShow != null)
            {
                OnWaveOnMessageShow(this, new WaveOnMessageShowEventArgs(BuildCountdownLevelMessage(), true));
            }
        }

        public void SetCanAct(bool canAct)
        {
            _canAct = canAct;
        }

        public bool HasWavesRemaining()
        {
            var hasWavesRemaining = _waveSOs.Any(w => w.WaveNumber > _currentWaveNumber);
            return hasWavesRemaining;
        }

        public void ShowWaveStartingMessage()
        {
            if (OnWaveOnMessageShow != null)
            {
                OnWaveOnMessageShow(this, new WaveOnMessageShowEventArgs(String.Format("Wave {0}", _currentWaveNumber + 1), false)); //StartNextWave is called on a delay, this is called before but then the wave number is still one too low so hacky fix
            }
        }

        public void StartNextWave()
        {
            _canSpawn = true;
            _currentWaveNumber++;
            _currentWave = _waveSOs.First(w => w.WaveNumber == _currentWaveNumber);
            _timePassedInWave = 0;
            _waveChunks.AddRange(_currentWave.WaveChunks);
            _canAct = true;

            if (OnWaveStarted != null)
            {
                OnWaveStarted(this, new WaveStartedEventArgs(_currentWave));
            }
        }

        public float GetCountdownLenght()
        {
            var countdownMessages = BuildCountdownLevelMessage();
            var countdownLengh = countdownMessages.Sum(cm => cm.Duration);

            return countdownLengh;
        }

        public void FinishCurrentWave()
        {
            _waveChunks.Clear();
            _canSpawn = false;

            _enemyController.KillAllEnemies();
        }

        private void Update()
        {
            if (_canAct == false) return;

            UpdateTime();
            SpawnEnemies();
            DeactivateOnWaveFinished();
        }

        private void Start()
        {
            _enemyController = FindObjectOfType<EnemyController>();
            _player = FindFirstObjectByType<Player>();
        }

        private void DeactivateOnWaveFinished()
        {
            if (AnyEnemiesRemaining()) return;
            if (AnyWaveChunksRemaining()) return;

            _canAct = false;

            if (OnWaveCompleted != null)
            {
                OnWaveCompleted(this, new WaveCompletedEventArgs(_currentWave, HasWavesRemaining()));
            }
        }

        private bool AnyEnemiesRemaining()
        {
            var enemies = FindObjectsOfType<Enemy>();
            var anyEnemiesRemaining = enemies.Any();

            return anyEnemiesRemaining;
        }

        private bool AnyWaveChunksRemaining()
        {
            return _waveChunks.Any();
        }

        private void UpdateTime()
        {
            _timePassedInWave += Time.deltaTime;
        }

        private void SpawnEnemies()
        {
            if (_waveChunks.Any() == false) return;

            var waveChunkToSpawn = GetWaveChunkToSpawn();
            if (waveChunkToSpawn == null) return;

            StartCoroutine(SpawnWaveChunk(waveChunkToSpawn));
            _waveChunks.Remove(waveChunkToSpawn);
        }

        private WaveChunkSO GetWaveChunkToSpawn()
        {
            var waveChunkToSpawnByTime = _waveChunks.OrderBy(wc => wc.StartSpawningAfterDelay).FirstOrDefault(wc => wc.StartSpawningAfterDelay < _timePassedInWave);
            if (waveChunkToSpawnByTime != null) return waveChunkToSpawnByTime;

            if (AnyEnemiesRemaining()) return null;

            var nextWavechunkToSpawn = _waveChunks.OrderBy(wc => wc.StartSpawningAfterDelay).FirstOrDefault();
            return nextWavechunkToSpawn;
        }

        private IEnumerator SpawnWaveChunk(WaveChunkSO waveChunkSO)
        {
            var spawnLocation = GetSpawnTransform();
            var randomPosition = GetRandomPosition(spawnLocation);
            var numberOfEnemiesToSpawn = GetNumberOfEnemiesToSpawn(waveChunkSO.NumberOfEnemiesToSpawn);
            for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                var enemyGameObject = Instantiate(waveChunkSO.EnemyPrefab, _enemiesParentGameObject.transform, true);
                randomPosition = waveChunkSO.WavePositionSpawnType == Enums.WavePositionSpawnType.Random ? GetRandomPosition(spawnLocation) : GetNearbyPosition(randomPosition);
                randomPosition = OffsetSpawnPositionToPlayer(randomPosition);
                randomPosition = spawnLocation.MovePositionToBeWithinSpawnbounds(randomPosition);
                enemyGameObject.transform.position = randomPosition;
                var enemy = enemyGameObject.GetComponent<Enemy>();

                if (enemy.IsBoss == false && enemy.IsMiniBoss == false) { enemyGameObject.transform.localScale = GetCurrentEnemyScale(); }

                if (enemy.IsBoss == false && enemy.IsMiniBoss == false) { ScaleEnemy(enemy); }

                enemy.OnCharacterDied += Enemy_OnCharacterDied;
                _enemyController.AddEnemy(enemy);

                yield return new WaitForSeconds(waveChunkSO.SpawnDelayBetweenEnemies);

                if (_canSpawn == false) break;
            }
        }

        private void ScaleEnemy(Enemy enemy)
        {
            enemy.ScaleHealth(GameController.instance.CurrentEnemyHealth);
            enemy.ScaleDamage(GameController.instance.CurrentEnemyDamage);
            enemy.ScaleReward(GameController.instance.CurrentEnemyReward);
        }

        private Vector3 GetCurrentEnemyScale()
        {
            var scale = GameController.instance.CurrentEnemyScale;
            var scaleVector = new Vector3(scale, scale, scale);
            return scaleVector;
        }

        public int GetNumberOfEnemiesToSpawn(int originalNumberOfEnemiesToSpawn)
        {
            if (originalNumberOfEnemiesToSpawn == 1) return originalNumberOfEnemiesToSpawn; //Prevent adding or removing (mini)bosses

            var statValue = StatsController.instance.GetStatTypeValue(Enums.StatType.EnemyCount);
            var numberOfEnemiesAfterStat = originalNumberOfEnemiesToSpawn + (int)statValue;
            numberOfEnemiesAfterStat = Mathf.Clamp(numberOfEnemiesAfterStat, 1, 50);
            return numberOfEnemiesAfterStat;
        }

        private Vector2 OffsetSpawnPositionToPlayer(Vector2 randomPosition)
        {
            var distanceToPlayer = Vector2.Distance(randomPosition, _player.transform.position);
            if (distanceToPlayer > _minOffsetToPlayer) return randomPosition;

            var diffEnemyToPlayer = randomPosition - (Vector2)_player.transform.position;
            var distanceRatioMultplier = _minOffsetToPlayer / distanceToPlayer;
            var wantedDiffToPlayer = diffEnemyToPlayer * distanceRatioMultplier;
            var randomPositionAtOffset = (Vector2)_player.transform.position + wantedDiffToPlayer;

            return randomPositionAtOffset;
        }

        private void Enemy_OnCharacterDied(object sender, CharacterDiedEventArgs e)
        {
            if (OnWaveOnEnemyKilledHandler != null)
            {
                OnWaveOnEnemyKilledHandler(this, new WaveOnEnemyKilledEventArgs());
            }
        }

        private Vector2 GetNearbyPosition(Vector2 randomPosition)
        {
            var xOffSet = Random.Range(-2f, 2f);
            var yOffSet = Random.Range(-2f, 2f);
            var nearbyPosition = new Vector2(randomPosition.x + xOffSet, randomPosition.y + yOffSet);
            return nearbyPosition;
        }

        private Vector2 GetRandomPosition(SpawnLocation spawnLocation)
        {
            var randomPosition = spawnLocation.GetRandomPositionWithinSpawnbounds();
            return randomPosition;
        }

        private SpawnLocation GetSpawnTransform()
        {
            var allSpawnLocations = _spawnLocationsParentGameObject.GetComponentsInChildren<SpawnLocation>().ToList();
            var spawnLocation = allSpawnLocations[Random.Range(0, allSpawnLocations.Count)];

            foreach (var currentSpawnLocation in allSpawnLocations)
            {
                if (currentSpawnLocation.IsPositionWithinSpawnBounds(_player.transform.position) == false) continue;

                spawnLocation = currentSpawnLocation;
                break;
            }

            return spawnLocation;
        }

        private List<WavePopupMessage> BuildCountdownLevelMessage()
        {
            List<WavePopupMessage> wavePopupMessage = new List<WavePopupMessage>();
            wavePopupMessage.Add(new WavePopupMessage("Level starting...", 1.5f, WavePopupMessageAudio.Ching));
            wavePopupMessage.Add(new WavePopupMessage("Get ready!", 1.5f, WavePopupMessageAudio.None));
            wavePopupMessage.Add(new WavePopupMessage("3", 1f, WavePopupMessageAudio.CountdownAudio));
            wavePopupMessage.Add(new WavePopupMessage("2", 1f, WavePopupMessageAudio.CountdownAudio));
            wavePopupMessage.Add(new WavePopupMessage("1", 1f, WavePopupMessageAudio.CountdownAudio));
            wavePopupMessage.Add(new WavePopupMessage("Go!", 1f, WavePopupMessageAudio.GoAudio));
            return wavePopupMessage;
        }

        private void OnCharacterDeath(object sender, EventArgs e)
        {
            if (OnWaveOnEnemyKilledHandler != null)
            {
                OnWaveOnEnemyKilledHandler(this, new WaveOnEnemyKilledEventArgs());
            }
        }

        public void Debug_FinishLevel()
        {
            _waveChunks.Clear();
            _waveSOs.Clear();
        }

        internal void Debug_FinishWave()
        {
            _waveChunks.Clear();
        }

        public delegate void WaveOnMessageShowHandler(object sender, WaveOnMessageShowEventArgs e);
        public event WaveOnMessageShowHandler OnWaveOnMessageShow;

        public delegate void WaveStartedHandler(object sender, WaveStartedEventArgs e);
        public event WaveStartedHandler OnWaveStarted;

        public delegate void WaveCompletedHandler(object sender, WaveCompletedEventArgs e);
        public event WaveCompletedHandler OnWaveCompleted;

        public delegate void WaveOnEnemyKilledHandler(object sender, WaveOnEnemyKilledEventArgs e);
        public event WaveOnEnemyKilledHandler OnWaveOnEnemyKilledHandler;
    }
}