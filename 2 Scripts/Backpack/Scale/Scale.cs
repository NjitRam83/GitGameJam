using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scale : MonoBehaviour
{
    [SerializeField] Transform _positiveSpawnPoint;
    [SerializeField] Transform _negativeSpawnPoint;
    [SerializeField] GameObject _positivePassivePrefab;
    [SerializeField] GameObject _negativePassivePrefab;
    [SerializeField] TextMeshProUGUI _positiveTextTotal;
    [SerializeField] TextMeshProUGUI _negativeTextTotal;
    private int _currentPositives;
    private int _currentNegatives;

    public void SpawnPositive(int positives)
    {
        for (int i = 0; i < positives; i++)
        {
            var newItem = Instantiate(_positivePassivePrefab, transform);
            newItem.transform.position = _positiveSpawnPoint.position;
            _currentPositives++;
        }
        _positiveTextTotal.SetText(string.Format("+{0}", _currentPositives));
    }

    public void SpawnNegative(int negatives)
    {
        for (int i = 0; i < negatives; i++)
        {
            var newItem = Instantiate(_negativePassivePrefab, transform);
            newItem.transform.position = _negativeSpawnPoint.position;
            _currentNegatives++;
        }
        _negativeTextTotal.SetText(string.Format("-{0}", _currentNegatives));
    }

}
