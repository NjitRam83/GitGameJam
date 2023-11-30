using UnityEngine;

namespace BackpackSurvivors.Combat.Movement
{
    public class StraightToTargetProjectileMovement : MonoBehaviour, IProjectileMovement
    {
        public Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame)
        {
            var newPosition = Vector2.MoveTowards(currentPosition, targetPosition, maxMovementPerFrame);
            return newPosition;
        }

        public bool ShouldRotateToFaceTarget()
        {
            return true;
        }
    }
}
