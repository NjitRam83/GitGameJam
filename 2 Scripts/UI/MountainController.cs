using BackpackSurvivors.Level.ScriptableObjects;
using BackpackSurvivors.MainGame;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI
{
    internal class MountainController : MonoBehaviour
    {

        [SerializeField] Image _route;
        [SerializeField] MountainLevelDetail _mountainLevelDetail;
        [SerializeField] MountainLevelPoint[] _levelPoints;
        float _startRouteProgress = 0f;
        float _endRouteProgress;
        bool _allowRouteDrawing = false;
        float _waitTime = 15.0f;

        bool _isShowingDetails = false;
        
        [SerializeField] Animator _detailsAnimator;

        private void Start()
        {
            // bind events on button press
            foreach (var levelPoint in _levelPoints)
            {
                levelPoint.OnGoLevel += SelectLevel;
            }

            for (int i = 0; i < GameController.instance.Levels.Count(); i++)
            {
                LevelSO level = GameController.instance.Levels[i];
                MountainLevelPoint point = _levelPoints.FirstOrDefault(x => x.LevelSO.LevelId == level.LevelId);

                // determine progress end point
                if (IsLevelUnlocked(level))
                {
                    _endRouteProgress = point.RouteProgress;
                    point.IsEnabled = true;
                    // determine progress start point
                    if (i > 0)
                    {
                        LevelSO prevLevel = GameController.instance.Levels[i-1];
                        MountainLevelPoint prevPoint = _levelPoints.FirstOrDefault(x => x.LevelSO.LevelId == prevLevel.LevelId);
                        prevPoint.UpdateVisualState();
                        _startRouteProgress = prevPoint.RouteProgress;
                    }
                }
                point.UpdateVisualState();
            }

            _route.fillAmount = _startRouteProgress;
            StartCoroutine(DrawRoute());
        }

        private bool IsLevelUnlocked(LevelSO levelSO)
        {
            if (levelSO.LevelId == 0) return true;

            var previousLevelId = levelSO.LevelId - 1;
            var previousLevelCompleted = GameController.instance.CompletedLevelIds.Contains(previousLevelId);

            //Debug.Log($"Level {levelSO.LevelId} unlocked: {previousLevelCompleted}");

            return previousLevelCompleted;
        }

        void Update()
        {
            if (_allowRouteDrawing && _route.fillAmount < _endRouteProgress)
            {
                _route.fillAmount += 1.0f / _waitTime * Time.deltaTime;
            }
        }

        private IEnumerator DrawRoute()
        {
            yield return new WaitForSeconds(0.1f);
            _allowRouteDrawing = true;
        }

        private void SelectLevel(object sender, GoLevelEventArgs e)
        {
            if (!_isShowingDetails)
            {
                _isShowingDetails = true;
                _detailsAnimator.SetBool("Visible",true);
            }

            foreach (var levelPoint in _levelPoints)
            {
                levelPoint.IsSelected = false;
                levelPoint.UpdateVisualState();
            }
            e.Level.IsSelected = true;
            e.Level.UpdateVisualState();
            _mountainLevelDetail.Init(e.Level);
        }
    }
}
