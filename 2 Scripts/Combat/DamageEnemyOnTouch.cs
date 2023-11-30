using BackpackSurvivors.Enemies;
using BackpackSurvivors.MainGame;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Combat
{
    [RequireComponent(typeof(DamageStats))]
    public class DamageEnemyOnTouch : DamageVisualizer
    {
        private List<Enemy> _enemiesDamaged = new List<Enemy>();
        private bool _beingDestroyed;
        [SerializeField] float _timeBeforeDestroy;

        public Character SourceCharacter { get; internal set; }
        public delegate void EnemyDamagedHandler(object sender, EnemyDamagedEventArgs e);
        public event EnemyDamagedHandler OnEnemyDamaged;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (HasDamagedMaxNumberOfEnemies()) return;

            var enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy == null) return;

            if(_enemiesDamaged.Contains(enemy)) return;

            var enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth == null) return;

            var damageStats = GetComponent<DamageStats>();

            var damage = GetDamage(damageStats, enemy, out bool wasCrit);
            enemyHealth.Damage(damage, null, 0f, 0f, Vector3.zero);

            ShowDamagePopup(damage, wasCrit, CharacterType.Enemy);
            _enemiesDamaged.Add(enemy);

            OnEnemyDamaged?.Invoke(this, new EnemyDamagedEventArgs(enemy));

            //Debug.Log($"Damaging enemy for {damage} damage");
            if (HasDamagedMaxNumberOfEnemies())
            {
                if (_beingDestroyed) return;
                StartCoroutine(StartDestroy());
            }
        }

        private bool HasDamagedMaxNumberOfEnemies()
        {
            var totalMaxNumberOfHits = GetMaxNumberOfHits();
            if (totalMaxNumberOfHits == 0) return false;
            var hasDamagedMaxNumberOfEnemies = _enemiesDamaged.Count >= totalMaxNumberOfHits;

            return hasDamagedMaxNumberOfEnemies;
        }

        private int GetMaxNumberOfHits()
        {
            var piercingStat = (int)StatsController.instance.GetStatTypeValue(StatType.Piercing);
            var damageStatsPiercingStats = GetComponent<DamageStats>().Piercing;
            damageStatsPiercingStats = Mathf.Clamp(damageStatsPiercingStats, 1, damageStatsPiercingStats);
            var totalMaxNumberOfHits = piercingStat + damageStatsPiercingStats;
            //Debug.Log($"max number of hits: {totalMaxNumberOfHits}");
            return totalMaxNumberOfHits;
        }

        private float GetDamage(DamageStats damageStats, Character enemy, out bool wasCrit)
        {
            var damage = DamageCalculationEngine.CalculateDamage(damageStats, SourceCharacter, enemy, out wasCrit);
            return damage;
        }

        private IEnumerator StartDestroy()
        {
            _beingDestroyed = true;
            BoxCollider2D boxColider = GetComponent<BoxCollider2D>();
            boxColider.enabled = false;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;

            yield return new WaitForSeconds(_timeBeforeDestroy);

            DoDestroy();
        }

        private void DoDestroy()
        {
            Destroy(gameObject);
        }
    }
}
