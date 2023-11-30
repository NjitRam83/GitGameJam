using BackpackSurvivors.Backpack;
using BackpackSurvivors.Combat;
using BackpackSurvivors.Enemies.Components.Attacks;
using BackpackSurvivors.Enemies.Components.Movement;
using BackpackSurvivors.MainGame;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BackpackSurvivors.Enemies.Boss
{
    [RequireComponent(typeof(EnemyMovement))]
    public class Boss : Enemy
    {
        [SerializeField] ShootProjectileAtPlayer _shootProjectile;
        [SerializeField] ShootAOE _shootAOE;
        [SerializeField] Animator _animator;


        [SerializeField] AudioSource[] _attack1;
        [SerializeField] AudioSource _attack2;
        [SerializeField] AudioSource _attack2Active;
        [SerializeField] AudioSource _death;
        [SerializeField] AudioSource[] _randomSounds;
        [SerializeField] int _randomSoundsMaxChance;
        [SerializeField] AudioSource[] _hurtSounds;
        [SerializeField] int _hurtSoundsMaxChance;

        private Player _player;
        private Health _health;

        private bool _isFiringAOE = false;

        private int _rotation_south = 0;
        private int _rotation_west = 2;
        private int _rotation_north = 4;
        private int _rotation_east = 6;

        public override void DoStart()
        {
            base.DoStart();
            _player = FindObjectOfType<Player>();
            _health = GetComponent<Health>();
            _health.OnDeath += OnDeath;
            _health.OnHit += OnDamage;
            StartCoroutine(DoRandomSoundCheck());
        }

        private void OnDeath()
        {
            AudioController.instance.PlayAudioSourceAsSfxClip(_death);
            _isFiringAOE = false;
            GetComponent<ShootAOE>().CanFire = false;
            GetComponent<ShootAOE>().enabled = false;
            GetComponent<ShootProjectileAtPlayer>().AllowFiring = false;
            GetComponent<ShootProjectileAtPlayer>().enabled = false;
            _healthbar.gameObject.SetActive(false);

            Debug.Log("BOSS DEAD");
        }

        public override float GetCorpseTime()
        {
            return 5.5f;
        }

        private void OnDamage()
        {
            int randomIntForSound = Random.Range(0, _randomSoundsMaxChance);
            if (randomIntForSound == 0)
            {
                int randomAudioClip = Random.Range(0, _hurtSounds.Length);
                AudioController.instance.PlayAudioSourceAsSfxClip(_hurtSounds[randomAudioClip]);
            }
        }

        public override void DoAwake()
        {
            base.DoAwake();
            _shootProjectile.OnAttackFired += _shootProjectileAtPlayer_OnAttackFired;
            _shootAOE.OnAttackFired += _shootAOE_OnAttackFired;
            _shootAOE.OnAttackCompleted += _shootAOE_OnAttackCompleted;

            EnemyController enemyController = FindObjectOfType<EnemyController>();
            enemyController.AddEnemy(this);
        }

        public override void DoUpdate()
        {
            SetFacingDirection();
        }


        private IEnumerator DoRandomSoundCheck()
        {
            while (_health.CurrentHealth > 0)
            {
                int randomIntForSound = Random.Range(0, _randomSoundsMaxChance);
                if (randomIntForSound == 0)
                {
                    int randomAudioClip = Random.Range(0, _randomSounds.Length);
                    //_randomSounds[randomAudioClip].Play();
                    AudioController.instance.PlayAudioSourceAsSfxClip(_randomSounds[randomAudioClip]);
                }
                yield return new WaitForSeconds(1f);
            }
        }

        private void SetFacingDirection()
        {
            float diffX = 0f;
            float diffY = 0f;
            int facingDirection = 0;

            HorizontalDirection horizontalDirection = HorizontalDirection.East;
            VerticalDirection verticalDirection = VerticalDirection.South;

            if (_player.transform.position.x > transform.position.x)
            {
                // player is east of boss
                diffX = _player.transform.position.x - transform.position.x;
                horizontalDirection = HorizontalDirection.East;
            }
            if (_player.transform.position.x < transform.position.x)
            {
                // player is west of boss
                diffX = transform.position.x - _player.transform.position.x;
                horizontalDirection = HorizontalDirection.West;
            }
            if (_player.transform.position.y > transform.position.y)
            {
                // player is north of boss
                diffY = _player.transform.position.y - transform.position.y;
                verticalDirection = VerticalDirection.North;
            }
            if (_player.transform.position.y < transform.position.y)
            {
                // player is south of boss
                diffY = transform.position.y - _player.transform.position.y;
                verticalDirection = VerticalDirection.South;
            }

            if (diffX > diffY)
            {
                // we are further in West or East direction compared to North or South direction from the player. 
                switch (horizontalDirection)
                {
                    case HorizontalDirection.East:
                        facingDirection = _rotation_east;
                        break;
                    case HorizontalDirection.West:
                        facingDirection = _rotation_west;
                        break;
                }
            }
            if (diffX < diffY)
            {
                // we are further in North or South direction compared to East or West direction from the player. 
                switch (verticalDirection)
                {
                    case VerticalDirection.North:
                        facingDirection = _rotation_north;
                        break;
                    case VerticalDirection.South:
                        facingDirection = _rotation_south;
                        break;
                }
            }

            _animator.SetInteger("FacingDirection", facingDirection);
        }

        private void _shootAOE_OnAttackFired(object sender, AttackFiredEventArgs e)
        {
            _isFiringAOE = true;
            _shootProjectile.AllowFiring = false;
            _animator.SetBool("Attack2", true);
            _animator.SetBool("IsSpinning", true);
            _animator.SetTrigger("StartSpin");
            //_attack2.Play();
            _attack2Active.Play();
            AudioController.instance.PlayAudioSourceAsSfxClip(_attack2);
        }
        private void _shootAOE_OnAttackCompleted(object sender, AttackCompletedEventArgs e)
        {
            _animator.SetBool("IsSpinning", false);
            _animator.SetBool("Attack2", false);
            _animator.SetTrigger("EndSpin");
            _shootProjectile.AllowFiring = true;
            _isFiringAOE = false;
            _attack2Active.Stop();
        }

        private void _shootProjectileAtPlayer_OnAttackFired(object sender, AttackFiredEventArgs e)
        {
            if (_isFiringAOE) return;

            int randomAudioClip = Random.Range(0, _attack1.Length);
            //_attack1[randomAudioClip].Play();
            AudioController.instance.PlayAudioSourceAsSfxClip(_attack1[randomAudioClip]);

            StartCoroutine(DoProjectileAnimation());
        }



        private IEnumerator DoProjectileAnimation()
        {
            _animator.SetBool("Attack1", true);
            yield return new WaitForSeconds(0.4f);
            _animator.SetBool("Attack1", false);
        }
    }
}
