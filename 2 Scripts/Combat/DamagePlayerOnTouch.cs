using BackpackSurvivors.Enemies;
using BackpackSurvivors.MainGame;
using MoreMountains.TopDownEngine;
using System;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Combat
{
    [RequireComponent(typeof(DamageStats))]
    public class DamagePlayerOnTouch : DamageVisualizer
    {
        public Character SourceCharacter { get; internal set; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var player = collision.gameObject.GetComponent<Player>();
            if (player == null) return;

            var playerHealth = player.GetComponent<Health>();
            if (playerHealth == null) return;

            var enemy = (Enemy)SourceCharacter;
            if (enemy == null || !enemy.IsAlive) return;

            var damageStats = GetComponent<DamageStats>();

            var damage = GetDamage(damageStats, player, out bool wasCrit);
            playerHealth.Damage(damage, null, 0f, 0f, Vector3.zero);

            if (damage > 0)
            {
                ShowDamagePopup(damage, wasCrit, CharacterType.Player);
            }
            
            if (OnDamageDoneHandler != null)
            {
                OnDamageDoneHandler(this, new DamageDoneEventArgs(damage));
            }
        }
    
        private float GetDamage(DamageStats damageStats, Character player, out bool wasCrit)
        {
            var damage = DamageCalculationEngine.CalculateDamage(damageStats, SourceCharacter, player, out wasCrit);
            return damage;
        }



        public delegate void DamageDoneHandler(object sender, DamageDoneEventArgs e);
        public event DamageDoneHandler OnDamageDoneHandler;
    }

    public class DamageDoneEventArgs : EventArgs
    {
        public float Damage;
        public DamageDoneEventArgs(float damage)
        {
            Damage = damage;
        }
    }
}
