using UnityEngine;

namespace BackpackSurvivors.Enemies.Components.Movement
{
    public class FollowPlayerAndPause : FollowPlayerMovement
    {
        [SerializeField] float _pauseMovementCooldown;
        [SerializeField] float _resumeMovementCooldown;

        private bool _movementIsPaused = false;
        private float _currentPauseMovementCooldown;
        private float _currentResumeMovementCooldown;

        public new void Init(float moveSpeed)
        {
            _currentPauseMovementCooldown = _resumeMovementCooldown;

            base.Init(moveSpeed);
        }

        private void Update()
        {
            DecreaseCooldown();
            SwitchMovementBasedOnCooldown();

            if (_movementIsPaused)
            {
                //Do nothing
            }
            else
            {
                MoveEnemy();
            }
        }

        private void DecreaseCooldown()
        {
            if (_movementIsPaused)
            {
                _currentResumeMovementCooldown -= Time.deltaTime;
            }
            else
            { 
                _currentPauseMovementCooldown-= Time.deltaTime;
            }
        }

        private void SwitchMovementBasedOnCooldown()
        {
            if (_currentPauseMovementCooldown <= 0)
            { 
                _movementIsPaused = true;
                _currentPauseMovementCooldown = _pauseMovementCooldown;
                return;
            }

            if (_currentResumeMovementCooldown <= 0)
            { 
                _movementIsPaused= false;
                _currentResumeMovementCooldown = _resumeMovementCooldown;
                return;
            }
        }
    }
}
