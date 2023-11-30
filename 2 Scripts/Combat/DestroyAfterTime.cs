using UnityEngine;

namespace BackpackSurvivors.Combat
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] float _maximumLifetime;

        private float _lifetime;

        private void Update()
        {
            _lifetime += Time.deltaTime;
            if(_lifetime >= _maximumLifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
