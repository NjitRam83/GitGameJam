using BackpackSurvivors.Combat;
using BackpackSurvivors.Combat.Projectiles;
using BackpackSurvivors.MainGame;
using UnityEngine;

namespace BackpackSurvivors.Enemies.Components.Attacks
{
    public class ShootProjectileAtPlayer : EnemyAttack
    {
        [SerializeField] GameObject _projectilePrefab;
        [SerializeField] float _cooldown;
        [SerializeField] float _moveSpeed;
        [SerializeField] float _maxRange;
        [SerializeField] Transform _spawnPoint;

        public bool AllowFiring = true;

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
            if (AllowFiring == false) return;

            if (_player == null || _player.isActiveAndEnabled == false) return; //TODO: Centralize in GameController ?

            Transform sourcePoint = _spawnPoint == null ? transform : _spawnPoint;


            GameObject projectile = Instantiate(_projectilePrefab, sourcePoint.position, Quaternion.identity);
            WeaponProjectile projectileGameObject = projectile.GetComponent<WeaponProjectile>();

            var damagePlayerOnTouch = projectileGameObject.GetComponent<DamagePlayerOnTouch>();
            damagePlayerOnTouch.SourceCharacter = GetDamageSourceCharacter();

            //TODO: Set projectile sprite and rotation
            projectileGameObject.ActivateWeaponProjectile(_player.transform, _player.transform.position, _moveSpeed, _maxRange);

            ResetCooldown();

            if (OnAttackFired != null)
            {
                OnAttackFired(this, new AttackFiredEventArgs());
            }
        }

        public delegate void AttackFiredHandler(object sender, AttackFiredEventArgs e);
        public event AttackFiredHandler OnAttackFired;
    }
}
