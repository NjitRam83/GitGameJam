using BackpackSurvivors.MainGame;
using BackpackSurvivors.Pickups;
using MoreMountains.TopDownEngine;
using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BackpackSurvivors.Enemies.Components
{
    [RequireComponent(typeof(Character))]
    public class DropPickupOnDeath : MonoBehaviour
    {
        [SerializeField] GameObject _pickupPrefab;
        [SerializeField] float _dropChance;
        [SerializeField] int _minAmount = 1;
        [SerializeField] int _maxAmount = 1;
        [SerializeField] bool _canScale = false;
        //[SerializeField] int _lootExplosionRange = 1;
        [SerializeField] bool _overrideLootExplosionRange = false;

        private Health _health;

        private void Awake()
        {
            RegisterDeathEvent();
        }

        private void RegisterDeathEvent()
        {
            _health = GetComponent<Health>();
            _health.OnDeath += OnDeath;

            if (_overrideLootExplosionRange)
            {
                Lootdrop lootdrop = _pickupPrefab.GetComponent<Lootdrop>();
                lootdrop.overrideSplashRange(15f);
            }
            
        }

        private void OnDeath()
        {
            var shouldDrop = GetShouldDrop();
            if (shouldDrop == false) return;

            int dropAmount = Random.Range(_minAmount, _maxAmount);

            for (int i = 0; i < dropAmount; i++)
            {
                var pickupGameObject = Instantiate(_pickupPrefab, transform.position, Quaternion.identity);

                var pickupParent = FindObjectOfType<PickupsParent>();
                if (pickupGameObject != null && pickupParent != null)
                {
                    pickupGameObject.transform.SetParent(pickupParent.transform, true);
                }
            }
        }

        public void ScalePickup(float modifier)
        {
            if (!_canScale) return;

            _minAmount = Convert.ToInt32(_minAmount * modifier);
            _maxAmount = Convert.ToInt32(_maxAmount * modifier);
        }

        private bool GetShouldDrop()
        {
            var chance = _dropChance + StatsController.instance.GetStatTypeValue(Shared.Enums.StatType.LuckPercentage);
            var shouldDrop = chance > Random.Range(0f, 1f);
            return shouldDrop;
        }
    }
}