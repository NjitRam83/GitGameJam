using BackpackSurvivors.Combat.Movement;
using BackpackSurvivors.MainGame;
using UnityEngine;

namespace BackpackSurvivors.Combat.Projectile_Movements
{
    public class CirclePlayerProjectileMovement : MonoBehaviour, IProjectileMovement
    {
        [SerializeField] float _rotationSpeed = 1f;
        [SerializeField] float _distanceFromPlayer = 2f;
        private Player _player;
        private float _angleToPlayer;

        private void Start()
        {
            _player = FindObjectOfType<Player>();
        }

        private void Update()
        {
            _angleToPlayer += _rotationSpeed;
        }

        public Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame)
        {
            var xDiff = _distanceFromPlayer * Mathf.Cos(_angleToPlayer);
            var yDiff = _distanceFromPlayer * Mathf.Sin(_angleToPlayer);
            var offsetToPlayer = new Vector2(xDiff, yDiff);
            var newPosition = (Vector2)_player.transform.position + offsetToPlayer;

            return newPosition;
        }

        public bool ShouldRotateToFaceTarget()
        {
            return false;
        }
    }
}
