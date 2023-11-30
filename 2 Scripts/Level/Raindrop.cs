using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BackpackSurvivors.Level
{
    internal class Raindrop : MonoBehaviour
    {
        float duration = 0.5f;
         
        public void Init(RaindropController callbackPoint)
        {
            StartCoroutine(StartHiding(callbackPoint));
        }

        IEnumerator StartHiding(RaindropController callbackPoint)
        {
            yield return new WaitForSeconds(duration);
            if (callbackPoint != null)
            {
                callbackPoint.RemovedDroplet();
            }            
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
