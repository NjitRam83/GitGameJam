using System;
using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        private List<Enemy> _enemies = new List<Enemy>();


        public Enemy GetEnemyWithinRange(Vector2 origin, float range)
        {
            var enemiesToIgnore = new List<Enemy>();
            return GetEnemyWithinRange(origin, range, enemiesToIgnore);
        }

        public Enemy GetEnemyWithinRange(Vector2 origin, float range, List<Enemy> enemiesToIgnore)
        {
            Enemy result = null;
            float closestEnemyRange = float.MaxValue;

            foreach (var enemy in _enemies)
            {
                if (ShouldSkipEnemy(enemy, enemiesToIgnore)) continue;

                var distanceToEnemy = Vector2.Distance(origin, enemy.transform.position);
                if (distanceToEnemy > range) continue;
                if (distanceToEnemy > closestEnemyRange) continue;

                result = enemy;
                closestEnemyRange = distanceToEnemy;
            }

            return result;
        }

        private void Start()
        {
            var allEnemiesInScene = FindObjectsOfType<Enemy>();
            _enemies.AddRange(allEnemiesInScene);
        }

        private bool ShouldSkipEnemy(Enemy enemy, List<Enemy> enemiesToIgnore)
        {
            if (enemy == null) return true;
            if (enemy.isActiveAndEnabled == false) return true;
            if (enemy.IsAlive == false) return true;
            if (enemiesToIgnore.Contains(enemy)) return true;

            return false;
        }

        public void AddEnemy(Enemy enemy)
        {
            _enemies.Add(enemy);
        }

        public void RemoveEnemy(Enemy enemy)
        {
            _enemies.Remove(enemy);
        }

        internal void KillAllEnemies()
        {
            foreach (var enemy in _enemies)
            {
                if (ShouldSkipEnemy(enemy, new List<Enemy>())) continue;

                enemy.Debug_Kill();
            }
        }
    }
}
