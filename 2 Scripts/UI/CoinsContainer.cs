using BackpackSurvivors.MainGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace BackpackSurvivors.UI
{
    internal class CoinsContainer : MonoBehaviour
    {
        [SerializeField] Transform _lostOrReceivedMoneyControllerContainer;
        [SerializeField] LostOrReceivedMoneyController _lostOrReceivedMoneyControllerPrefab;
        [SerializeField] TMPro.TextMeshProUGUI CoinsText;

        private void Start()
        {
            MoneyController.instance.OnMoneyChanged += Instance_OnMoneyChanged;
            UpdateCoins(MoneyController.instance.Coins, 0);
        }

        private void Instance_OnMoneyChanged(object sender, MoneyChangedEventArgs e)
        {
            UpdateCoins(e.NewAmount, e.ChangedValue);
        }


        private void UpdateCoins(int newValue, int changedAmount)
        {
            CoinsText.SetText(newValue.ToString());

            LostOrReceivedMoneyController _lostOrReceivedMoneyController = Instantiate(_lostOrReceivedMoneyControllerPrefab, _lostOrReceivedMoneyControllerContainer);
            _lostOrReceivedMoneyController.Init(changedAmount);
        }

        private void OnDestroy()
        {
            MoneyController.instance.OnMoneyChanged -= Instance_OnMoneyChanged;
        }
    }
}
