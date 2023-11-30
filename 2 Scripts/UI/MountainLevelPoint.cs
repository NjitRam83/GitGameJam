using BackpackSurvivors.Level.ScriptableObjects;
using BackpackSurvivors.MainGame;
using BackpackSurvivors.UI.Tooltip;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

namespace BackpackSurvivors.UI
{
    public class MountainLevelPoint : MonoBehaviour
    {
        [SerializeField] int _levelId;
        public LevelSO LevelSO => GameController.instance.GetLevelById(_levelId);
        [SerializeField] Button _button;
        [SerializeField] Image _selectedBorder;
        [SerializeField] Image _bossBorder;
        [SerializeField] Image _background;
        [SerializeField] Sprite _disabledBackground;
        [SerializeField] Sprite _enabledBackground;
        [SerializeField] TextMeshProUGUI _levelNumber;
        [SerializeField] GameObject _common;
        [SerializeField] GameObject _uncommon;
        [SerializeField] GameObject _rare;
        [SerializeField] GameObject _epic;
        [SerializeField] GameObject _legendary;
        [SerializeField] GameObject _mythic;


        [SerializeField] AudioSource _buttonClickSound;

        [SerializeField] TooltipTrigger _tooltipTrigger;

        [SerializeField] public float RouteProgress;
        [SerializeField] bool _isBoss;

        public bool IsEnabled;
        public bool IsSelected;

        public void UpdateVisualState()
        {
            _button.interactable = IsEnabled;
            if (LevelSO != null)
            {
                _levelNumber.SetText((LevelSO.LevelId + 1).ToString());
            }

            if (_bossBorder != null)
            {
                _bossBorder.gameObject.SetActive(_isBoss && IsEnabled);
            }
            



            if (IsEnabled)
            {
                _background.sprite = _enabledBackground;

                _common.SetActive(LevelSO.ItemQualitiesToUnlock.Contains(Shared.Enums.ItemQuality.Common));
                _uncommon.SetActive(LevelSO.ItemQualitiesToUnlock.Contains(Shared.Enums.ItemQuality.Uncommon));
                _rare.SetActive(LevelSO.ItemQualitiesToUnlock.Contains(Shared.Enums.ItemQuality.Rare));
                _epic.SetActive(LevelSO.ItemQualitiesToUnlock.Contains(Shared.Enums.ItemQuality.Epic));
                _legendary.SetActive(LevelSO.ItemQualitiesToUnlock.Contains(Shared.Enums.ItemQuality.Legendary));
                _mythic.SetActive(LevelSO.ItemQualitiesToUnlock.Contains(Shared.Enums.ItemQuality.Mythic));

                ColorUtility.TryParseHtmlString("#844E02", out Color myColor);
                _levelNumber.color = myColor;
            }
            else
            {
                _background.sprite = _disabledBackground;
                _levelNumber.color = Color.gray;
            }

            _selectedBorder.gameObject.SetActive(IsSelected);


            _tooltipTrigger.ToggleEnabled(IsEnabled);
            _tooltipTrigger.SetContent(LevelSO.LevelName, "Click to show details");
        }

        public void GoLevel()
        {
            if (OnGoLevel != null && IsEnabled)
            {
                Debug.Log($"Go Level {_levelId}");
                AudioController.instance.PlayAudioSourceAsSfxClip(_buttonClickSound);
                OnGoLevel(this, new GoLevelEventArgs(this));
            }
        }

        public delegate void GoLevelHandler(object sender, GoLevelEventArgs e);
        public event GoLevelHandler OnGoLevel;
    }
}
