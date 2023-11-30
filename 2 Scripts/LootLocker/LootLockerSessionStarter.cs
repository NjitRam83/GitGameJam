using BackpackSurvivors.Shared;
using LootLocker.Requests;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.LootLocker
{
    internal class LootLockerSessionStarter : MonoBehaviour
    {
        [SerializeField] TMP_InputField _playernameInputField;

        private void Start()
        {
            StartSession();
        }
        private void StartSession()
        {
            try
            {
                LootLockerSDKManager.StartGuestSession((response) =>
                {
                    if (!response.success)
                    {
                        Debug.Log("error starting LootLocker session");

                        return;
                    }

                    PlayerPrefs.SetString(Constants.PlayerPrefsVariables.PlayerId, response.player_id.ToString());
                    FillPlayerName();
                    Debug.Log("successfully started LootLocker session");
                });
            }
            catch (System.Exception e)
            {
                Debug.Log($"{e.InnerException}");
            }
        }

        private void FillPlayerName()
        {
            try
            {
                LootLockerSDKManager.GetPlayerName((response) =>
                {
                    if (!response.success) { return; }
                    if (response.name.Length == 0) { return; }

                    _playernameInputField.text = response.name;
                    Debug.Log($"Player name: {response.name}");
                });
            }
            catch (System.Exception e)
            {
                Debug.Log($"{e.InnerException}");
            }
        }

        public void SetNameInLootLocker()
        {
            if (_playernameInputField.text.Equals(string.Empty)) return;

            try
            {
                LootLockerSDKManager.SetPlayerName(_playernameInputField.text, (response) =>
                {
                    if (!response.success)
                    {
                        Debug.Log(response.text);
                    }
                });
            }
            catch (System.Exception e)
            {
                Debug.Log($"{e.InnerException}");
            }
        }
    }
}
