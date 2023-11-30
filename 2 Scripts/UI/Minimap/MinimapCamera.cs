using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BackpackSurvivors.UI.Minimap
{
    public class MinimapCamera : MonoBehaviour
    {
        [SerializeField] public Transform Player;

        // Update is called once per frame
        void LateUpdate()
        {
            if (Player != null)
            {
                Vector3 newPos = Player.position;
                newPos.z = transform.position.z;
                transform.position = newPos;
            }
        }
    }
}