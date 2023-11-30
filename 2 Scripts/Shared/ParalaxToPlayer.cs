using BackpackSurvivors.MainGame;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxToPlayer : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _playerCamera;

    [SerializeField] float start = -32f;
    [SerializeField] float end = 32f;
    [SerializeField] float startMinimized = 5;
    [SerializeField] bool includeY;


    // Update is called once per frame
    void Update()
    {
        float i = Mathf.InverseLerp(start, end, _playerCamera.transform.position.x);
        float modX = i * startMinimized * 2;

        float modY = transform.position.y;

        if (includeY)
        {
            modY = _playerCamera.transform.position.y;
        }
        transform.position = new Vector3(_playerCamera.transform.position.x + (startMinimized/2) - modX, modY, transform.position.z);
    }
}
