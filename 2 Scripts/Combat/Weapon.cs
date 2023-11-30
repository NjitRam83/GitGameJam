using BackpackSurvivors.Combat.Projectiles;
using BackpackSurvivors.Enemies;
using BackpackSurvivors.Items.ScriptableObjectSSS;
using BackpackSurvivors.MainGame;
using System;
using System.Collections;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;
using Random = UnityEngine.Random;

namespace BackpackSurvivors.Combat
{
    public class Weapon : MonoBehaviour
    {
        public float CurrentCooldown => _currentCooldown;
        public float Cooldown => _cooldown;
        public BackpackItemSO BackpackItem => _backpackItemSO;
        public Guid UniqueWeaponKey;
        public string Name => _name;

        private string _name;
        private float _cooldown;
        private float _range;
        private float _currentCooldown;
        private float _attackDelay;
        private EnemyController _enemyController;
        private BackpackItemSO _backpackItemSO;
        private Player _player;
        private Vector2 _randomAttackDirection;
        private AttackType _attackType;
        private AudioSource _audioSourceAttack;

        private void Start()
        {
            _enemyController = FindObjectOfType<EnemyController>();
        }

        public void Init(BackpackItemSO backpackItemSO, Player player, AttackType attackType, float attackDelay)
        {
            _backpackItemSO = backpackItemSO;
            _player = player;
            _attackType = attackType;

            _attackDelay = attackDelay;

            _name = _backpackItemSO.Name;
            _cooldown = _backpackItemSO.ItemStats.StatValues[StatType.CooldownTime];
            _range = _backpackItemSO.ItemStats.StatValues[StatType.WeaponRange];
            _currentCooldown = _cooldown;

            _audioSourceAttack = GetComponent<AudioSource>();
            _audioSourceAttack.clip = backpackItemSO.AudioOnAttack;
            _audioSourceAttack.volume = backpackItemSO.AudioVolumeOnAttack;

            UniqueWeaponKey = Guid.NewGuid();
        }

        public void DecreaseCooldown(float time)
        {
            _currentCooldown -= time;
        }

        public bool CanAttack()
        {
            return _currentCooldown <= 0;
        }

        bool _didPerformAttack;
        bool _attemptingAttack = false;
        public void Attack()
        {
            _didPerformAttack = false;
            if (!_attemptingAttack)
            {
                switch (_attackType)
                {
                    case AttackType.AttackClosestEnemy: StartCoroutine(AttackClosestEnemy()); break;
                    case AttackType.AttackNorth: StartCoroutine(AttackNorth()); break;
                    case AttackType.AttackEast: StartCoroutine(AttackEast()); break;
                    case AttackType.AttackSouth: StartCoroutine(AttackSouth()); break;
                    case AttackType.AttackWest: StartCoroutine(AttackWest()); break;
                    case AttackType.AttackRandomDirection: StartCoroutine(AttackRandomDirection()); break;
                    case AttackType.AttackCursor: StartCoroutine(AttackCursor()); break;
                    case AttackType.AttackRandomDirectionEachAttack: StartCoroutine(AttackRandomDirectionEachAttack()); break;
                }
            }
        }

        private IEnumerator AttackRandomDirectionEachAttack()
        {
            _attemptingAttack = true;
            yield return new WaitForSeconds(_attackDelay);
            var randomAttackDirection = GetRandomDirection();
            var randomTargetPosition = transform.position + (Vector3)randomAttackDirection;
            var targetPosition = GetTargetAtMaxRange(randomTargetPosition, _range);
            AttackTargetInStraightLine(targetPosition);
            _didPerformAttack = true;
            _attemptingAttack = false;
            HandleAfterAttack();
        }

        public void HandleAfterAttack()
        {
            if (_didPerformAttack == false) return;

            if (OnWeaponAttack != null)
            {
                OnWeaponAttack(this, new WeaponAttackEventArgs(this));
            }

            _currentCooldown = _cooldown;
            if (_audioSourceAttack.clip != null)
            {
                //_audioSourceAttack.PlayScheduled(-0.5);
                AudioController.instance.PlayAudioSourceAsSfxClip(_audioSourceAttack);
            }
        }
        private Vector2 GetRandomDirection()
        {
            var x = Random.Range(-1f, 1f);
            var y = Random.Range(-1f, 1f);
            return new Vector2(x, y);
        }

        private IEnumerator AttackCursor()
        {
            _attemptingAttack = true;
            yield return new WaitForSeconds(_attackDelay);
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var targetPosition = GetTargetAtMaxRange(mousePosition, _range);
            AttackTargetInStraightLine(targetPosition);
            _didPerformAttack = true;
            _attemptingAttack = false;
            HandleAfterAttack();
        }

        private IEnumerator AttackRandomDirection()
        {
            _attemptingAttack = true;
            yield return new WaitForSeconds(_attackDelay);
            if (_randomAttackDirection == Vector2.zero)
            {
                _randomAttackDirection = GetRandomDirection();
            }

            var randomTargetPosition = transform.position + (Vector3)_randomAttackDirection;
            var targetPosition = GetTargetAtMaxRange(randomTargetPosition, _range);
            AttackTargetInStraightLine(targetPosition);
            _didPerformAttack = true;
            _attemptingAttack = false;
            HandleAfterAttack();
        }

        private IEnumerator AttackNorth()
        {
            _attemptingAttack = true;
            yield return new WaitForSeconds(_attackDelay);
            var targetPosition = transform.position + new Vector3(0, _range);
            AttackTargetInStraightLine(targetPosition);
            _didPerformAttack = true;
            _attemptingAttack = false;
            HandleAfterAttack();
        }

        private IEnumerator AttackEast()
        {
            _attemptingAttack = true;
            yield return new WaitForSeconds(_attackDelay);
            var targetPosition = transform.position + new Vector3(_range, 0);
            AttackTargetInStraightLine(targetPosition);
            _didPerformAttack = true;
            _attemptingAttack = false;
            HandleAfterAttack();
        }

        private IEnumerator AttackSouth()
        {
            _attemptingAttack = true;
            yield return new WaitForSeconds(_attackDelay);
            var targetPosition = transform.position + new Vector3(0, -1 * _range);
            AttackTargetInStraightLine(targetPosition);
            _didPerformAttack = true;
            _attemptingAttack = false;
            HandleAfterAttack();
        }

        private IEnumerator AttackWest()
        {
            _attemptingAttack = true;
            yield return new WaitForSeconds(_attackDelay);
            var targetPosition = transform.position + new Vector3(-1 * _range, 0);
            AttackTargetInStraightLine(targetPosition);
            _didPerformAttack = true;
            _attemptingAttack = false;
            HandleAfterAttack();
        }

        private IEnumerator AttackClosestEnemy()
        {
            _attemptingAttack = true;
            yield return new WaitForSeconds(_attackDelay);

            var enemy = _enemyController.GetEnemyWithinRange(transform.position, _range);
            if (enemy == null)
            {
                _didPerformAttack = false;
                _attemptingAttack = false;
            }
            else
            {
                if (!_didPerformAttack)
                {
                    var targetPosition = GetTargetAtMaxRange(enemy.transform.position, _range);
                    AttackTargetInStraightLine(targetPosition);
                    _didPerformAttack = true;
                    _attemptingAttack = false;
                }
            }
            HandleAfterAttack();
        }

        private GameObject InitProjectile()
        {
            var weaponProjectileGameObject = Instantiate(_backpackItemSO.ProjectilePrefab, transform.position, Quaternion.identity);

            var damageOnTouch = weaponProjectileGameObject.GetComponent<DamageEnemyOnTouch>();
            damageOnTouch.SourceCharacter = _player;

            var weaponProjectile = weaponProjectileGameObject.GetComponent<WeaponProjectile>();
            _backpackItemSO.InintExternalDamageStats(weaponProjectile.DamageStats);

            return weaponProjectileGameObject;
        }

        private void AttackTargetInStraightLine(Vector2 targetPosition)
        {
            var weaponProjectileGameObject = InitProjectile();
            var weaponProjectile = weaponProjectileGameObject.GetComponent<WeaponProjectile>();

            weaponProjectile.ActivateWeaponProjectile(_player.transform, targetPosition, 20f, _range); //TODO: Store projectile speed in projectile, make possible to apply stats ?
        }

        private Vector2 GetTargetAtMaxRange(Vector2 targetPosition, float range)
        {
            var targetToCurrentOffset = targetPosition - (Vector2)transform.position;
            var distanceToTarget = Vector2.Distance(targetPosition, transform.position);
            var rangeMultiplier = range / distanceToTarget;
            var maxRangeToCurrentOffset = rangeMultiplier * targetToCurrentOffset;
            var targetAtMaxRange = (Vector2)transform.position + maxRangeToCurrentOffset;
            return targetAtMaxRange;
        }

        public delegate void WeaponAttackedHandler(object sender, WeaponAttackEventArgs e);
        public event WeaponAttackedHandler OnWeaponAttack;
    }
}
