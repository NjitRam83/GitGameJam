using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Minimap
{
    public class Minimap : MonoBehaviour
    {
        [SerializeField] MinimapCamera _minimapCamera;
        [SerializeField] bool enableOnStartup = true;
        [SerializeField] Animator animator;
        [SerializeField] Toggle _toggle;

        private void Start()
        {
            animator.SetBool("Open", true);
        }

        public void ToggleMinimap()
        {
            animator.SetBool("Open", _toggle.isOn);
        }

        public void SetCameraToFollow(Transform player)
        {
            _minimapCamera.Player = player;
            gameObject.SetActive(enableOnStartup);
        }
    }
}