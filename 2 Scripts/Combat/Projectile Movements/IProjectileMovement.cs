using UnityEngine;

namespace BackpackSurvivors.Combat.Movement
{
    internal interface IProjectileMovement
    {
        Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame);
        bool ShouldRotateToFaceTarget();
    }
}
