using BackpackSurvivors.DailyChallenges;
using BackpackSurvivors.Shared;
using LootLocker.Requests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.LootLocker
{
    public class LootLockerController : MonoBehaviour
    {
        public static LootLockerController instance;

        public void SubmitScore(int score, string leaderboardKey)
        {
            try
            {
                var playerId = PlayerPrefs.GetString(Constants.PlayerPrefsVariables.PlayerId);
                LootLockerSDKManager.SubmitScore(playerId, score, leaderboardKey, (response) =>
                {
                    if (response.success)
                    {
                        Debug.Log("Succes submitting score");
                    }
                    else
                    {
                        Debug.Log($"Failed: {response.text}");
                    }
                });
            }
            catch (System.Exception e)
            {
                Debug.Log($"{e.InnerException}");
            }
        }

        public bool SetCheckmarkIfChallengeCompleted(string leaderboardKey, TextMeshProUGUI buttonText, Image buttonImage, Sprite checkmarkSprite)
        {
            var result = false;

            LootLockerSDKManager.GetScoreList(leaderboardKey, 100, (response) =>
            {
                if (response.success)
                {
                    for (int i = 0; i < response.items.Length; i++)
                    {
                        var currentItem = response.items[i];
                        if (currentItem.player.id.ToString().Equals(PlayerPrefs.GetString(Constants.PlayerPrefsVariables.PlayerId)) == false) continue;

                        var finished = currentItem.score == 4;
                        if (finished)
                        {
                            buttonText.text = string.Empty;
                            buttonImage.sprite = checkmarkSprite;
                        }
                    }
                }
            });

            return result;
        }

        private string GetPlayername(LootLockerLeaderboardMember currentItem)
        {
            if (currentItem.player.name.Length > 0) { return currentItem.player.name; }
            return currentItem.player.public_uid;
        }

        private void Awake()
        {
            SetupSingleton(out bool continueAwake);
            if (continueAwake == false) return;
        }

        private void SetupSingleton(out bool continueAwake)
        {
            if (instance != null)
            {
                Destroy(gameObject);
                continueAwake = false;
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            continueAwake = true;
        }
    }
}
