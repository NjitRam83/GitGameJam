using UnityEngine;

namespace BackpackSurvivors.Enemies.Components.Movement
{
    public class FollowPlayerUntilRange : EnemyMovement
    {
        [SerializeField] float _distanceToKeepFromPlayer;

        public new void Init(float moveSpeed)
        {
            base.Init(moveSpeed);
        }

        internal void MoveEnemy()
        {
            var newPosition = Vector2.MoveTowards(transform.position, _player.transform.position, MoveSpeed * Time.deltaTime);
            var distanceToPlayer = Vector2.Distance(newPosition, _player.transform.position);
            if (distanceToPlayer < _distanceToKeepFromPlayer) return;

            base.MoveEnemy(newPosition);
        }

        private void Update()
        {
            MoveEnemy();
        }
    }
}
