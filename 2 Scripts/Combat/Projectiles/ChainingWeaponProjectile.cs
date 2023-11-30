using BackpackSurvivors.Combat.Projectiles.ChainLightning;
using BackpackSurvivors.Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.Combat.Projectiles
{
    public class ChainingWeaponProjectile : WeaponProjectile
    {
        private EnemyController _enemyController;
        private List<Enemy> _damagedEnemies = new List<Enemy>();

        ChainLightningLineRenderer _renderer;
        [SerializeField] ChainLightningLineRenderer _rendererPrefab;
        [SerializeField] ChainLightningPoint _rendererPointPrefab;

        public override void ActivateWeaponProjectile(Transform sourceTransform, Vector2 targetPosition, float speed, float maxRange)
        {
            base.ActivateWeaponProjectile(sourceTransform, targetPosition, speed, maxRange);

            RegisterControllers();
            RegisterEnemyDamaged();
            _renderer = Instantiate(_rendererPrefab, transform);
            ChainLightningPoint newPoint = Instantiate(_rendererPointPrefab, sourceTransform);
            newPoint.DoParticles(false);
            _renderer.AddPoint(newPoint);
        }

        private void RegisterControllers()
        {
            _enemyController = FindObjectOfType<EnemyController>();
        }

        private void RegisterEnemyDamaged()
        {
            var damageEnemyOnTouch = GetComponent<DamageEnemyOnTouch>();
            if (damageEnemyOnTouch == null) return;

            damageEnemyOnTouch.OnEnemyDamaged += DamageEnemyOnTouch_OnEnemyDamaged;
        }

        private void UnregisterEnemyDamaged()
        {
            var damageEnemyOnTouch = GetComponent<DamageEnemyOnTouch>();
            if (damageEnemyOnTouch == null) return;

            damageEnemyOnTouch.OnEnemyDamaged -= DamageEnemyOnTouch_OnEnemyDamaged;
        }

        private void DamageEnemyOnTouch_OnEnemyDamaged(object sender, EnemyDamagedEventArgs e)
        {
            var newPoint = Instantiate(_rendererPointPrefab, e.DamagedEnemy.transform);
            newPoint.DoParticles(true);
            _renderer.AddPoint(newPoint);

            _damagedEnemies.Add(e.DamagedEnemy);
            var nextTargetEnemy = _enemyController.GetEnemyWithinRange(transform.position, _maxRange, _damagedEnemies);
            if (nextTargetEnemy == null)
            {
                StartCoroutine(StartDestroying(1f));
                return;
            }

            _targetPosition = nextTargetEnemy.transform.position;
        }

        private IEnumerator StartDestroying(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            UnregisterEnemyDamaged();
        }
    }
}
