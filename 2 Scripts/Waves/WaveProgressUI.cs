using BackpackSurvivors.Waves;
using BackpackSurvivors.Waves.ScriptableObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveProgressUI : MonoBehaviour
{
    [SerializeField] GameObject barContainer;
    [SerializeField] Image barImagePrefab;
    [SerializeField] TextMeshProUGUI WaveText;
    [SerializeField] TextMeshProUGUI killCounterText;

    private Dictionary<int, Image> _chunks = new Dictionary<int, Image>();
    private Dictionary<int, int> _chunkTotals = new Dictionary<int, int>();
    private int _totalEnemiesKilledThisWave;

    WavesController _wavesController;

    private void Start()
    {
        _wavesController = FindObjectOfType<WavesController>();

        if (_wavesController == null) return;
        _wavesController.OnWaveOnEnemyKilledHandler += OnWaveOnEnemyKilledHandler;
        _wavesController.OnWaveStarted += _wavesController_OnWaveStarted;
        _wavesController.OnWaveCompleted += _wavesController_OnWaveCompleted;
    }

    private void _wavesController_OnWaveCompleted(object sender, WaveCompletedEventArgs e)
    {
        for (int i = 0; i < _chunkTotals.Count; i++)
        {
            _chunks[i].fillAmount = 1f;
        }
    }

    private void _wavesController_OnWaveStarted(object sender, WaveStartedEventArgs e)
    {
        WaveText.SetText(string.Format("Wave {0}", e.Wave.WaveNumber));
        Init(e.Wave);
    }

    private void OnWaveOnEnemyKilledHandler(object sender, WaveOnEnemyKilledEventArgs e)
    {
        _totalEnemiesKilledThisWave++;
        killCounterText.SetText(_totalEnemiesKilledThisWave.ToString());
        Progress();
    }

    public void Init(WaveSO wave)
    {
        DestroyExistingChunkImages();
        
        _totalEnemiesKilledThisWave = 0;
        _chunks.Clear();
        _chunkTotals.Clear();
        killCounterText.SetText(_totalEnemiesKilledThisWave.ToString());

        int chunkIndex = 0;
        foreach (WaveChunkSO chunk in wave.WaveChunks)
        {
            Image chunkImage = Instantiate(barImagePrefab, barContainer.transform);
            chunkImage.fillAmount = 0f;
            _chunks.Add(chunkIndex, chunkImage);
            var numberOfEnemiesInChunk = _wavesController.GetNumberOfEnemiesToSpawn(chunk.NumberOfEnemiesToSpawn);
            _chunkTotals.Add(chunkIndex, numberOfEnemiesInChunk);
            chunkIndex++;
        }
    }

    private void Progress()
    {
        int remainingKillsToHandle = _totalEnemiesKilledThisWave;
        for (int i = 0; i < _chunkTotals.Count; i++)
        {
            if (_chunkTotals[i] < remainingKillsToHandle)
            {
                _chunks[i].fillAmount = 1f; // is this needed? Since we do this every kill no backtracking is needed?
                remainingKillsToHandle -= _chunkTotals[i];
            }
            else if(remainingKillsToHandle > 0)
            {
                _chunks[i].fillAmount = (float)remainingKillsToHandle / _chunkTotals[i];
                remainingKillsToHandle = 0;
            }
        }
    }

    private void DestroyExistingChunkImages()
    {
        foreach (var chunk in _chunks)
        {
            var chunkImage = chunk.Value;
            chunkImage.gameObject.SetActive(false);
            Destroy(chunkImage.gameObject);
        }
    }
}
