using UnityEngine;

namespace BackpackSurvivors.DEBUG
{
    public class RaycastLogger : MonoBehaviour
    {
        private Vector3 _mousePosition;

        private void Update()
        {
            _mousePosition = Input.mousePosition;
        }
        private void FixedUpdate()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
                LogRaycast();
                //Debug.Break();
            //}
        }

        private void LogRaycast()
        {
            var layerMask = 1 << LayerMask.NameToLayer("RaycastHittable");

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                //Debug.DrawRay(Input.mousePosition, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                Debug.Log("Did Hit");
                Debug.Log(hit.collider.gameObject.name);
            }
            else
            {
                //Debug.DrawRay(ray);
            }
        }
    }
}
