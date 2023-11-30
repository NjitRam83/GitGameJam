using BackpackSurvivors.MainGame;
using System;
using System.Collections;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI
{
    public class MountainLevelDetail : MonoBehaviour
    {
        [SerializeField] AudioSource _buttonClickSound;
        [SerializeField] TextMeshProUGUI _title;
        [SerializeField] TextMeshProUGUI _description;
        [SerializeField] LevelImage[] _levelImages;
        [SerializeField] TextMeshProUGUI _score;
        [SerializeField] TextMeshProUGUI _time;
        [SerializeField] TextMeshProUGUI _coins;
        [SerializeField] Image _mainStar;
        [SerializeField] Image _leftStar;
        [SerializeField] Image _rightStar;
        MountainLevelPoint _point;

        public void Init(MountainLevelPoint point)
        {
            _point = point;

            _title.SetText(point.LevelSO.LevelName);

            foreach (var levelImage in _levelImages)
            {
                levelImage.gameObject.SetActive(false);
            }

            StartCoroutine(ShowImage(_point));
        }

        private IEnumerator ShowImage(MountainLevelPoint point)
        {
            yield return new WaitForSeconds(0.3f);

            _description.gameObject.SetActive(true);
            var descriptionText = point.LevelSO.Description;
            descriptionText = descriptionText.Replace("[NL]", Environment.NewLine);
            _description.SetText(descriptionText);

            if (_levelImages.Length >= point.LevelSO.LevelId)
            {
                _levelImages[point.LevelSO.LevelId].gameObject.SetActive(true);
            }
        }

        public void GoLevel()
        {
            AudioController.instance.PlayAudioSourceAsSfxClip(_buttonClickSound);
            GameController.instance.NextLevelIdToLoad = _point.LevelSO.LevelId;
            if (!GameController.instance.CanDoFirstTimeLoaded)
            {
                GameController.instance.DoFirstTimeLoad();
            }
            else
            {
                GameController.instance.StartNextLevel();
            }
        }
    }
}