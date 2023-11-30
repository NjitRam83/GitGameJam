using UnityEngine;

namespace BackpackSurvivors.Combat.Projectiles.ChainLightning
{
    public class ChainLightningPoint : MonoBehaviour
    {

        [SerializeField] ParticleSystem _particleSystem;
        public void DoParticles(bool particles)
        {
            if (_particleSystem != null)
            {
                _particleSystem.gameObject.SetActive(particles);
            }
        }
        public Vector3 GetPosition()
        {
            if (transform == null) return new Vector3();

            return transform.position;
        }
    }
}