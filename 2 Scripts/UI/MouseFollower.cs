using UnityEngine;

namespace BackpackSurvivors.UI
{
    internal class MouseFollower : MonoBehaviour
    {
        void Update()
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z - transform.position.z)));
            newPos.z = transform.position.z;
            transform.position = newPos;
        }
    }
}