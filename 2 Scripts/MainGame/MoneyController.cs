using BackpackSurvivors.UI;
using System;
using UnityEngine;

namespace BackpackSurvivors.MainGame
{


    public class MoneyController : MonoBehaviour
    {
        [SerializeField] private int _initialCoins;
        [SerializeField] AudioSource pickupAudio;

        private int _coins;
        public static MoneyController instance;

        public int Coins => _coins;

        public void GainCoins(int numberOfCoins)
        {
            _coins += numberOfCoins;

            //pickupAudio.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(pickupAudio);
            if (OnMoneyChanged != null)
            {
                OnMoneyChanged(this, new MoneyChangedEventArgs(_coins, numberOfCoins));
            }
        }

        public bool TrySpendCoins(int numberOfCoins)
        {
            if (_coins < numberOfCoins) return false;

            _coins -= numberOfCoins;
            if (OnMoneyChanged != null)
            {
                OnMoneyChanged(this, new MoneyChangedEventArgs(_coins, -numberOfCoins));
            }
            
            return true;
        }

        public bool CanAfford(int cost)
        { 
            return cost <= _coins;
        }

        private void Awake()
        {
            SetupSingleton();
        }

        private void SetupSingleton()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                InitController();
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void InitController()
        {
            _coins = _initialCoins;
        }

        public delegate void MoneyChangedHandler(object sender, MoneyChangedEventArgs e);
        public event MoneyChangedHandler OnMoneyChanged;
    }

    public class MoneyChangedEventArgs : EventArgs
    {

        public MoneyChangedEventArgs(int newAmount, int changedValue)
        {
            NewAmount = newAmount;
            ChangedValue = changedValue;
        }

        public int NewAmount { get; }
        public int ChangedValue { get; }
    }
}
