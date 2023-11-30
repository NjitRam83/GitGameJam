using BackpackSurvivors.MainGame;
using MoreMountains.TopDownEngine;
using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Pickups
{
    public class CoinPickup : Lootdrop
    {
        [SerializeField] private bool _blockRetrievalOnLevelCompletion;


        const float _coinMoveSpeed = 0.15f;
        private int _coinValue = 1;
        private bool _moveToPlayer = false;
        private bool _fromCompletion = false;
        private Player _player;

        public void SetMoveToPlayer(bool moveToPlayer, bool fromCompletion)
        {
            _moveToPlayer = moveToPlayer;
            _fromCompletion = fromCompletion;
        }

        public void SetCoinValue(int coinValue)
        {
            _coinValue = coinValue;
        }

        private void Start()
        {
            _player = FindObjectOfType<Player>();
        }

        private void Update()
        {
            DoUpdate();
        }

        public override void DoUpdate()
        {
            base.DoUpdate();
            MoveCoinToPlayer(_fromCompletion);
        }

        private void MoveCoinToPlayer(bool fromCompletion)
        {
            
            if (_moveToPlayer == false) return; 

            if (fromCompletion && _blockRetrievalOnLevelCompletion == true) return;

            var newPosition = Vector2.MoveTowards(transform.position, _player.transform.position, _coinMoveSpeed);
            transform.position = newPosition;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var pickupRadius = collision.gameObject.GetComponent<PickupRadius>();
            if (pickupRadius == null) return;

            SetMoveToPlayer(true, false);

            StartCoroutine(DoCoinPickup(pickupRadius.ActualRadius));
        }

        private IEnumerator DoCoinPickup(float distance)
        {
            yield return new WaitForSeconds(distance/20f);
            FindObjectOfType<MoneyController>().GainCoins(_coinValue);
            Destroy(gameObject);
        }
    }
}