using BackpackSurvivors.MainGame;
using MoreMountains.TopDownEngine;
using System;
using UnityEngine;

namespace BackpackSurvivors.Enemies.Components.Movement
{
    public abstract class EnemyMovement : MonoBehaviour
    {
        [SerializeField] bool _rotateTowardsPlayer = true;
        public float MoveSpeed;
        public EventHandler OnCanMoveChanged;
        internal Player _player;

        internal bool CanMove => _canMove;

        private bool _canMove = true;

        public void SetCanMove(bool canMove)
        {
            _canMove = canMove;
            OnCanMoveChanged?.Invoke(this, null);
        }

        public void Init(float moveSpeed)
        {
            MoveSpeed = moveSpeed;
        }

        private void Start()
        {
            _player = FindObjectOfType<Player>();
        }

        internal void RotateTowardsPlayer()
        {
            if (_rotateTowardsPlayer == false) return;

            var playerToTheRightOfEnemy = _player.transform.position.x > transform.position.x;
            var xScaleMultiplier = playerToTheRightOfEnemy ? 1 : -1;
            transform.localScale = new Vector3(xScaleMultiplier * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        internal void MoveEnemy(Vector2 newPosition)
        {
            transform.position = newPosition;
            RotateTowardsPlayer();
        }
    }
}
