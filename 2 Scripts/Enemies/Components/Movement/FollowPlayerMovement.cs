using UnityEngine;

namespace BackpackSurvivors.Enemies.Components.Movement
{
    public class FollowPlayerMovement : EnemyMovement
    {
        internal void MoveEnemy()
        {
            if (_player == null || _player.isActiveAndEnabled == false) return;
            if(CanMove == false) return;

            var newPosition = Vector2.MoveTowards(transform.position, _player.transform.position, MoveSpeed * Time.deltaTime);

            base.MoveEnemy(newPosition);
        }

        private void Update()
        {
            MoveEnemy();
        }
    }
}
