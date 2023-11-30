using BackpackSurvivors.Combat;
using BackpackSurvivors.Enemies.Components;
using BackpackSurvivors.Enemies.Components.Attacks;
using BackpackSurvivors.Enemies.Components.Movement;
using BackpackSurvivors.Enemies.ScriptableObjects;
using BackpackSurvivors.MainGame;
using BackpackSurvivors.Waves;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;
using Random = UnityEngine.Random;

namespace BackpackSurvivors.Enemies
{
    [HasSortingLayer("_layer")]
    [RequireComponent(typeof(EnemyMovement))]
    public class Enemy : Character
    {
        [SerializeField] EnemyPropertiesSO _enemyPropertiesSO;
        [SerializeField] float _minSpeed;
        [SerializeField] float _maxSpeed;
        [SerializeField] Animator _enemyAnimator;
        [SerializeField] CircleCollider2D _collidingCircleCollider;
        [SerializeField] AudioSource _audioSourceOnHit;
        [SerializeField] AudioSource _audioSourceOnDeath;
        [SerializeField] internal MMProgressBar _healthbar;
        [SerializeField] bool _isRangedEnemy;

        public bool IsAlive => _isAlive;

        private bool _isAlive = true;
        private EnemyMovement _enemyMovement;

        public bool IsBoss => _enemyPropertiesSO.IsBoss;
        public bool IsMiniBoss => _enemyPropertiesSO.IsMiniBoss;

        private void Awake()
        {
            DoAwake();
        }

        public virtual void DoAwake()
        {
            SetCharacterType(CharacterType.Enemy);
            HookupCharacterDeath();
            HookupCharacterHit();
            HookupEnemyMoving();
            _audioSourceOnHit.clip = _enemyPropertiesSO.AudioOnHit;
            _audioSourceOnDeath.clip = _enemyPropertiesSO.AudioOnDeath;
        }

        private void Update()
        {
            DoUpdate();
        }

        public virtual void DoUpdate()
        {

        }

        private void HookupEnemyMoving()
        {
            _enemyMovement = GetComponent<EnemyMovement>();
            _enemyMovement.OnCanMoveChanged += EnemyMovement_OnCanMoveChanged;
        }

        private void EnemyMovement_OnCanMoveChanged(object sender, EventArgs e)
        {
            _enemyAnimator.SetBool("IsMoving", _enemyMovement.CanMove);
        }

        private void InitProperties()
        {
            InitHealth();
            InitDamageStats();
            InitDamagePlayerOnTouch();
            InitMovement();
            InitRewards();
            if (_isRangedEnemy)
            {
                InitRangedAttack();
            }
        }

        private void InitRewards()
        {
            DropPickupOnDeath[] drops = GetComponentsInChildren<DropPickupOnDeath>();
            foreach (var drop in drops)
            {
                drop.ScalePickup(rewardModifier);
            }
        }

        private void InitRangedAttack()
        {
            ShootProjectileAtPlayer shootProjectileAtPlayer = GetComponentInChildren<ShootProjectileAtPlayer>();
            if (shootProjectileAtPlayer == null) return;
            shootProjectileAtPlayer.OnAttackFired += ShootProjectileAtPlayer_OnAttackFired;
        }

        private void ShootProjectileAtPlayer_OnAttackFired(object sender, AttackFiredEventArgs e)
        {
            _enemyAnimator.SetTrigger("OnAttack");
        }

        private void Start()
        {
            DoStart();
        }

        public virtual void DoStart()
        {
            StatsController.instance.ApplyStatsToCharacter(this, CharacterType.Enemy);
            InitProperties();
        }

        private void InitDamagePlayerOnTouch()
        {
            var damagePlayerOnTouch = GetComponentInChildren<DamagePlayerOnTouch>();
            damagePlayerOnTouch.OnDamageDoneHandler += DamagePlayerOnTouch_OnDamageDoneHandler;
            damagePlayerOnTouch.SourceCharacter = this;
        }

        private void DamagePlayerOnTouch_OnDamageDoneHandler(object sender, DamageDoneEventArgs e)
        {
            if (_enemyAnimator.parameters.Any(x => x.name == "OnAttack"))
            {
                _enemyAnimator.SetTrigger("OnAttack");
            }
        }

        private void InitDamageStats()
        {
            var damageStats = GetComponentInChildren<DamageStats>();
            damageStats.MinDamage = _enemyPropertiesSO.MinDamageOnTouch * damageModifier;
            damageStats.MaxDamage = _enemyPropertiesSO.MaxDamageOnTouch * damageModifier;
            damageStats.CritChance = _enemyPropertiesSO.CritChance;
            damageStats.CritMultiplier = _enemyPropertiesSO.CritMultiplier;
        }

        private float healthModifier = 1f;
        internal void ScaleHealth(float currentEnemHealthMod)
        {
            healthModifier = currentEnemHealthMod;
        }

        private float damageModifier = 1f;
        internal void ScaleDamage(float currentEnemyDamageMod)
        {
            damageModifier = currentEnemyDamageMod;
        }

        private float rewardModifier = 1f;
        internal void ScaleReward(float currentEnemyRewardMod)
        {
            rewardModifier = currentEnemyRewardMod;
        }

        private void InitHealth()
        {
            var healthMultiplier = 1 + GetStatValue(StatType.Health);
            var health = GetComponent<Health>();

            health.MaximumHealth = _enemyPropertiesSO.Hp * healthMultiplier * healthModifier;
            health.SetHealth(health.MaximumHealth);

            health.UpdateHealthBar(true);
        }

        private void InitMovement()
        {
            var speedMultiplier = 1 + GetStatValue(StatType.SpeedPercentage);
            var moveSpeed = Random.Range(_enemyPropertiesSO.MinimumMoveSpeed, _enemyPropertiesSO.MaximumMoveSpeed);
            moveSpeed *= speedMultiplier;
            moveSpeed = Mathf.Clamp(moveSpeed, _minSpeed, _maxSpeed);
            var movement = GetComponent<EnemyMovement>();
            movement.Init(moveSpeed);
        }

        public override void CharacterDied()
        {
            _isAlive = false;
            base.CharacterDied();

            if (_collidingCircleCollider != null)
            {
                _collidingCircleCollider.enabled = false;
            }

            ClearWavechunksOnBosskill();
            AudioController.instance.PlayAudioSourceAsSfxClip(_audioSourceOnDeath);
            GetComponent<EnemyMovement>().SetCanMove(false);
            _enemyAnimator.SetBool("IsDead", true);
            _enemyAnimator.SetTrigger("OnDeath");
            StartCoroutine(AfterDeath());
        }

        private void ClearWavechunksOnBosskill()
        {
            if (IsBoss || IsMiniBoss)
            { 
                FindObjectOfType<WavesController>().FinishCurrentWave();
            }
        }

        private IEnumerator AfterDeath()
        {
            yield return new WaitForSeconds(GetCorpseTime());
            gameObject.SetActive(false);
            Destroy(this.gameObject);
        }

        public virtual float GetCorpseTime()
        {
            return 1.5f;
        }

        public override void CharacterHit()
        {
            _enemyAnimator.SetTrigger("OnHurt");
            AudioController.instance.PlayAudioSourceAsSfxClip(_audioSourceOnHit);
        }


        public void Debug_Kill()
        {
            var health = GetComponent<Health>();
            health.Kill();
        }
        public void Debug_TakeHit()
        {
            var health = GetComponent<Health>();
            health.Damage(1f, gameObject, 0f, 0f, new Vector3(), null);
        }
        public void Debug_Attack()
        {
            DamagePlayerOnTouch_OnDamageDoneHandler(this, new DamageDoneEventArgs(1));
        }


    }
}
