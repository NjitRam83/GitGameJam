using BackpackSurvivors.MainGame;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BackpackSurvivors.Pickups
{
    public class HealthPickup : Lootdrop
    {
        [SerializeField] float _healthValue;
        [SerializeField] int _maxNumberOfHealthPickupsInScene;

        private void Start()
        {
            var numberOfHealthPickupsInScene = FindObjectsOfType<HealthPickup>().Count();
            if (numberOfHealthPickupsInScene > _maxNumberOfHealthPickupsInScene)
            { 
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var pickupRadius = collision.gameObject.GetComponent<PickupRadius>();
            if (pickupRadius == null) return;

            var player = FindObjectOfType<Player>();
            if (player == null) return;

            if (player.GetHealth().CurrentHealth == player.GetHealth().MaximumHealth) return;

            player.Heal(_healthValue);
            Destroy(gameObject);
        }
    }
}
