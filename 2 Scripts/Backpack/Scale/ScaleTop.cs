using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTop : MonoBehaviour
{
    Vector3 originalLocation;

    // Start is called before the first frame update
    void Start()
    {
        originalLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = originalLocation;
    }
}
