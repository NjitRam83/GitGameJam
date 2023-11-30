using BackpackSurvivors.Combat;
using BackpackSurvivors.Combat.Projectiles;
using BackpackSurvivors.MainGame;
using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Enemies.Components.Attacks
{
    public class ShootAOE : EnemyAttack
    {
        [SerializeField] float _cooldown;
        [SerializeField] GameObject _projectilePrefab;
        [SerializeField] float _moveSpeed;
        [SerializeField] float _maxRange;
        [SerializeField] int _fireAmount;
        [SerializeField] float _cooldownPerBolt;
        public bool CanFire = true;
        bool _isAOEIng;

        private Player _player;

        private void Awake()
        {
            Init(_cooldown);
        }

        private void Start()
        {
            _player = FindObjectOfType<Player>();
        }

        private void Update()
        {
            DecreaseCooldown(Time.deltaTime);
            Attack();
        }

        private void Attack()
        {
            if (CanAttackBasedOnCooldown() == false) return;
            if (_isAOEIng == true) return;
            if (_player == null || _player.isActiveAndEnabled == false) return; //TODO: Centralize in GameController ?


            if (OnAttackFired != null)
            {
                OnAttackFired(this, new AttackFiredEventArgs());
            }
            _isAOEIng = true;
            StartCoroutine(DoFullAttack(transform));


        }

        private Vector3 RandomVector(float min, float max)
        {
            var x = Random.Range(min, max);
            var y = Random.Range(min, max);
            var z = Random.Range(min, max);
            return new Vector3(x, y, z);
        }

        private void CreateSingleProjectile(Transform sourcePoint, Vector3 targetPoint)
        {
            var projectileGameObject = Instantiate(_projectilePrefab, sourcePoint.position, Quaternion.identity);
            var damagePlayerOnTouch = projectileGameObject.GetComponent<DamagePlayerOnTouch>();
            damagePlayerOnTouch.SourceCharacter = GetDamageSourceCharacter();

            var projectile = projectileGameObject.GetComponent<WeaponProjectile>();
            projectile.ActivateWeaponProjectile(_player.transform, targetPoint, _moveSpeed, _maxRange);
        }

        private IEnumerator DoFullAttack(Transform sourcePoint)
        {
            for (int i = 0; i < _fireAmount; i++)
            {
                if (CanFire)
                {
                    CreateSingleProjectile(sourcePoint, RandomVector(-360, 360));
                    yield return new WaitForSeconds(_cooldownPerBolt);
                }
            }

            if (OnAttackCompleted != null)
            {
                OnAttackCompleted(this, new AttackCompletedEventArgs());
            }

            ResetCooldown();
            _isAOEIng = false;
        }

        public delegate void AttackFiredHandler(object sender, AttackFiredEventArgs e);
        public event AttackFiredHandler OnAttackFired;

        public delegate void AttackCompletedHandler(object sender, AttackCompletedEventArgs e);
        public event AttackCompletedHandler OnAttackCompleted;
    }
}
