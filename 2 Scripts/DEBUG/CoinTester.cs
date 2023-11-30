using BackpackSurvivors.Pickups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTester : MonoBehaviour
{
    [SerializeField] CoinPickup coinPrefab;
    [SerializeField] Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCoin()
    {
        Instantiate(coinPrefab, parent);
    }
}
