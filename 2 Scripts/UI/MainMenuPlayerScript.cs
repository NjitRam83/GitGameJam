using UnityEngine;

namespace BackpackSurvivors.UI
{
    public class MainMenuPlayerScript : MonoBehaviour
    {
        [SerializeField] Animator animator;

        public void TargetNewGameButton()
        {
            GoIdle();
            animator.SetBool("NewGame", true);
        }

        public void TargetHowToPlayButton()
        {
            GoIdle();
            animator.SetBool("HowToPlay", true);
        }

        public void TargetSettingsButton()
        {
            GoIdle();
            animator.SetBool("Settings", true);
        }

        public void TargetCreditsButton()
        {
            GoIdle();
            animator.SetBool("Credits", true);
        }
        
        public void GoIdle()
        {
            Debug.Log("Going Idle");

            animator.SetBool("NewGame", false);
            animator.SetBool("Settings", false);
            animator.SetBool("Credits", false);
            animator.SetBool("HowToPlay", false);
        }
    }
}