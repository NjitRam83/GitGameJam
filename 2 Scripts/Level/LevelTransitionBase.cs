using BackpackSurvivors.MainGame;
using BackpackSurvivors.Shared;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BackpackSurvivors.Level
{
    public abstract class LevelTransitionBase : MonoBehaviour
    {

        public abstract void StartLevelTransition();

        public void DisablePlayerMovement(Player player)
        {
            player.GetComponent<CharacterMovement>().AbilityPermitted = false;
            player.gameObject.layer = LayerMask.NameToLayer(Constants.Layers.NoColission);
        }
    }
}
