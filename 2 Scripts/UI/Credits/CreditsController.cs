using BackpackSurvivors.MainGame;
using BackpackSurvivors.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BackpackSurvivors.UI.Credits
{
    internal class CreditsController : MonoBehaviour
    {
        public TextMeshProUGUI TitleTextToFade;
        public GameObject CreditsText;
        public GameObject CreditsTextTarget;
        private Vector3 targetPosition;
        public float TimeToSpend;
        public CreditsLightScript[] CreditItemsToHighlight;

        void Start()
        {
            //float additionalMovement = (540 * 2 )+ (540/2); // screen height * 2 (double up) + screenheight / 2 to get the middle;

            //Debug.Log(height);
            //float height = ((RectTransform)CreditsText.transform).rect.height;
            Debug.Log(targetPosition);
            //AudioController.instance.StopMusicClip();
            StartCoroutine(FadeTitleToBlack());
            StartCoroutine(MoveOverSeconds(CreditsText, TimeToSpend));

        }

        private IEnumerator FadeTitleToBlack()
        {
            while (TitleTextToFade.color.a > 0)
            {
                var newColor = TitleTextToFade.color;
                newColor.a -= 0.3f * Time.deltaTime;
                TitleTextToFade.color = newColor;
                yield return null;
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < CreditItemsToHighlight.Length; i++)
            {
                if (!CreditItemsToHighlight[i].Shown && elapsedTime > CreditItemsToHighlight[i].timeStampToShow)
                {
                    CreditItemsToHighlight[i].Show();
                }
            }
        }

        public void GoMainMenu()
        {
            SceneManager.LoadScene(Constants.Scenes.MainMenuScene);
        }

        public void GoMountain()
        {
            SceneManager.LoadScene(Constants.Scenes.MountainScalingScene);
        }

        float elapsedTime = 0;
        public IEnumerator MoveOverSeconds(GameObject objectToMove, float seconds)
        {
            Vector3 startingPos = objectToMove.transform.position;
            Vector3 end = new Vector3(0, 0, 0);
            while (elapsedTime < seconds)
            {
                end = CreditsTextTarget.transform.position;
                objectToMove.transform.position = Vector3.Lerp(startingPos, end, (float)(elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            objectToMove.transform.position = end;
        }
    }
}
    