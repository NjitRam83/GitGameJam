using BackpackSurvivors.LootLocker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.DailyChallenges
{
    public class DailyChallengeButton : MonoBehaviour
    {
        [SerializeField] Sprite _checkmarkSprite;

        private Button _button;
        private DailyChallengeSO _dailyChallengeSO;
        private DailyChallengeController _dailyChallengeController;
        private TextMeshProUGUI _buttonText;
        private Image _buttonImage;


        public void SetDailyChallengeSO(DailyChallengeSO dailyChallengeSO)
        {
            _dailyChallengeSO = dailyChallengeSO;
            _buttonText.text = dailyChallengeSO.DayNumber.ToString();
            SetCheckmarkIfFinished();
        }

        private void SetCheckmarkIfFinished()
        {
            LootLockerController.instance.SetCheckmarkIfChallengeCompleted(_dailyChallengeSO.LeaderboardKey, _buttonText, _buttonImage, _checkmarkSprite);
        }

        private void Awake()
        {
            RegisterButton();
            _buttonText = GetComponentInChildren<TextMeshProUGUI>();
            _buttonImage = GetComponent<Image>();
        }

        private void Start()
        {
            _dailyChallengeController = FindObjectOfType<DailyChallengeController>();
        }

        private void RegisterButton()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Button_Click);
        }

        private void Button_Click()
        {
            _dailyChallengeController.ChallengeSelected(_dailyChallengeSO);
        }
    }
}
