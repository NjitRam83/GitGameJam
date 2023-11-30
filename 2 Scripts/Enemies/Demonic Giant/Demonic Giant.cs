using BackpackSurvivors.Backpack;
using BackpackSurvivors.Combat;
using BackpackSurvivors.Enemies.Components.Attacks;
using BackpackSurvivors.Enemies.Components.Movement;
using BackpackSurvivors.MainGame;
using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;
using Random = UnityEngine.Random;

namespace BackpackSurvivors.Enemies.DemonicGiant
{
    [RequireComponent(typeof(EnemyMovement))]
    public class DemonicGiant : Enemy
    {

        [SerializeField] Animator _animator;
        private Player _player; 
        
        private int _rotation_south = 0;
        private int _rotation_west = 1;
        private int _rotation_north = 2;
        private int _rotation_east = 3;

        public override void DoStart()
        {
            base.DoStart();
            _player = FindObjectOfType<Player>();
        }


        public override void DoAwake()
        {
            base.DoAwake();
        }

        public override void DoUpdate()
        {
            base.DoUpdate();
            SetFacingDirection();
        }

        private void SetFacingDirection()
        {
            float diffX = 0f;
            float diffY = 0f;
            int facingDirection = 0;

            HorizontalDirection horizontalDirection = HorizontalDirection.East;
            VerticalDirection verticalDirection = VerticalDirection.South;

            if (_player == null) return;

            if (_player.transform.position.x > transform.position.x)
            {
                // player is east of boss
                diffX = _player.transform.position.x - transform.position.x;
                horizontalDirection = HorizontalDirection.East;
            }
            if (_player.transform.position.x < transform.position.x)
            {
                // player is west of boss
                diffX = transform.position.x - _player.transform.position.x;
                horizontalDirection = HorizontalDirection.West;
            }
            if (_player.transform.position.y > transform.position.y)
            {
                // player is north of boss
                diffY = _player.transform.position.y - transform.position.y;
                verticalDirection = VerticalDirection.North;
            }
            if (_player.transform.position.y < transform.position.y)
            {
                // player is south of boss
                diffY = transform.position.y - _player.transform.position.y;
                verticalDirection = VerticalDirection.South;
            }

            if (diffX > diffY)
            {
                // we are further in West or East direction compared to North or South direction from the player. 
                switch (horizontalDirection)
                {
                    case HorizontalDirection.East:
                        facingDirection = _rotation_east;
                        break;
                    case HorizontalDirection.West:
                        facingDirection = _rotation_west;
                        break;
                }
            }
            if (diffX < diffY)
            {
                // we are further in North or South direction compared to East or West direction from the player. 
                switch (verticalDirection)
                {
                    case VerticalDirection.North:
                        facingDirection = _rotation_north;
                        break;
                    case VerticalDirection.South:
                        facingDirection = _rotation_south;
                        break;
                }
            }

            _animator.SetInteger("FacingDirection", facingDirection);
        }
    }
}
