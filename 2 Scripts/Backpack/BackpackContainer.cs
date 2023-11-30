using BackpackSurvivors.MainGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackContainer : MonoBehaviour
{

    public static BackpackContainer instance;

    private void Awake()
    {
        SetupSingleton();
    }

    private void SetupSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
