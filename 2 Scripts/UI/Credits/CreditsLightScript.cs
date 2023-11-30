using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BackpackSurvivors.UI.Credits
{
    public class CreditsLightScript : MonoBehaviour
    {
        public float timeStampToShow;
        [SerializeField] Light2D _light;
        private float minIntensity = 0f;
        private float maxIntensity = 2f;
        public float intensityIncreases;
        public bool Shown = false;

        // Start is called before the first frame update
        void Start()
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (_shouldShow)
            {
                if (_showingStage == 0)
                {
                    // change intensity up
                    if (_light.intensity < maxIntensity)
                    {
                        // raise up
                        _light.intensity += (intensityIncreases * Time.deltaTime);
                    }
                    else
                    {
                        _showingStage = 1;
                    }
                }
                else if (_showingStage == 1)
                {
                    // start wait timer
                    StartCoroutine(WaitFor(3f, 3));
                    _showingStage = 2;
                }
                else if (_showingStage == 2)
                {
                    // do nothing - wait
                }
                else if (_showingStage == 3)
                {
                    // starting hiding
                    if (_light.intensity >= minIntensity)
                    {
                        // lower down
                        _light.intensity = _light.intensity - (intensityIncreases * Time.deltaTime);
                    }
                    else
                    {
                        Shown = true;
                        gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        _showingStage = 4;
                    }
                }
            }
        }

        private bool _shouldShow;
        private int _showingStage = 0; //0 = showing, 1 = waiting, 3 = hiding
        public void Show()
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            _shouldShow = true;
        }

        IEnumerator WaitFor(float waitTime, int nextStage)
        {
            yield return new WaitForSeconds(3f);
            _showingStage = nextStage;
        }
    }
}
    