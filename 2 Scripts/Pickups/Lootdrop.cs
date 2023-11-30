using UnityEngine;

namespace BackpackSurvivors.Pickups
{
    public abstract class Lootdrop : MonoBehaviour
    {
        [Header("Lootdrop")]
        [SerializeField] float splashRange = 1f;

        // loot splosion
        private Transform ObjectTransform;
        private float delay = 0f;
        private float pastTime = 0f;
        private float when = 0.3f;
        private Vector3 off;

        private bool _done;

        public virtual string GetItemName()
        {
            return string.Empty;
        }

        public void overrideSplashRange(float newSplashRange)
        {
            splashRange = newSplashRange;
        }

        void Awake()
        {
            ObjectTransform = GetComponent<Transform>();
            off = new Vector3(Random.Range(-System.Math.Abs(splashRange), System.Math.Abs(splashRange)), off.y, off.z);
            off = new Vector3(off.x, Random.Range(-System.Math.Abs(splashRange), System.Math.Abs(splashRange)), off.z);
        }

        void Update()
        {
            DoUpdate();
        }

        public virtual void DoUpdate()
        {
            if (_done) return;

            if (when >= delay)
            {
                pastTime = Time.deltaTime;

                ObjectTransform.position += off * Time.deltaTime;
                delay += pastTime;
            }
            else
            {
                _done = true;
            }
        }
    }

}