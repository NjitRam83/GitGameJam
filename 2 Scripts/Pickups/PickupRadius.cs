using UnityEngine;

namespace BackpackSurvivors.Pickups
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class PickupRadius : MonoBehaviour
    {
        [SerializeField] float _baseRadius;
        public float ActualRadius;
        private CircleCollider2D _circleCollider;

        private void Start()
        {
            UpdatePickupRadius(0);
        }

        public void UpdatePickupRadius(float radiusIncrease)
        {
            ActualRadius = _baseRadius * (1 + radiusIncrease);
            _circleCollider.radius = ActualRadius;
        }

        private void Awake()
        {
            _circleCollider = GetComponent<CircleCollider2D>();
        }
    }
}
