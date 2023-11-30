using BackpackSurvivors.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BackpackSurvivors.UI
{
    public class StoryController : MonoBehaviour
    {
        [SerializeField] AudioSource _buttonClick;
        public void GoNext()
        {
            SceneManager.LoadScene(Constants.Scenes.MountainScalingScene, LoadSceneMode.Single);
        }
    }
}