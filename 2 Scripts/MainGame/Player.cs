using BackpackSurvivors.Pickups;
using BackpackSurvivors.UI;
using Cinemachine;
using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;
using Random = UnityEngine.Random;
using Weapon = BackpackSurvivors.Combat.Weapon;

namespace BackpackSurvivors.MainGame
{
    public class Player : Character
    {
        [SerializeField] GameObject _weaponsParent;
        [SerializeField] float _baseSpeed;
        [SerializeField] float _minSpeed;
        [SerializeField] float _maxSpeed;
        [SerializeField] float _baseHealth;
        [SerializeField] public GameObject MinimapPoint;
        [SerializeField] Animator _animator;
        [SerializeField] Rigidbody2D _rigidbody;

        [SerializeField] AudioSource[] _hurtAudio;
        [SerializeField] AudioSource _dyingAudio;
        [SerializeField] AudioSource _healedAudio;
        [SerializeField] FlashImage _playerHurtImage;
        [SerializeField] CinemachineVirtualCamera _virtualCamera; 
        private float _shakeTimer;
        private bool _handlePlayerInput = true;

        private PickupRadius _pickupRadius;
        private Health _health;
        private CharacterMovement _characterMovement;
        private CharacterDash2D _characterDash2D;
        [SerializeField] TrailRenderer trailRenderer;

        public EventHandler OnPlayerDeath;

        public GameObject WeaponsParent => _weaponsParent;

        public void ClearWeapons()
        {
            foreach (var weapon in _weaponsParent.GetComponentsInChildren<Weapon>())
            {
                Destroy(weapon);
            }
        }

        public void InitHealth()
        {
            var healthModifier = 1 + GetStatValue(StatType.Health);
            var newHealth = _baseHealth * healthModifier;
            _health.SetHealth(newHealth);
            _health.MaximumHealth = newHealth;
        }

        public void SetPlayerRunningUntilReset(bool isRunning, bool isRunningRight)
        {
            _animator.SetBool("IsRunning", isRunning);
            _handlePlayerInput = !isRunning;
            var xScale = isRunningRight ? 1 : -1;
            transform.localScale = new Vector3(xScale, 1f, 1f);
        }

        private void Awake()
        {
            SetCharacterType(CharacterType.Player);
            HookupCharacterDeath();
            HookupCharacterHit();
            RegisterComponents();
            OnStatsUpdated += Character_OnStatsUpdated;
        }

        private void RegisterComponents()
        {
            _pickupRadius = GetComponentInChildren<PickupRadius>();
            _characterMovement = GetComponent<CharacterMovement>();
            _health = GetComponent<Health>();
            _characterDash2D = GetComponent<CharacterDash2D>();
            _characterDash2D.OnDashStarted += _characterDash2D_OnDashStarted;
        }

        private void _characterDash2D_OnDashStarted(object sender, DashbarStartedEventArgs e)
        {
            trailRenderer.enabled = true;
            StartCoroutine(EndDashTrail());
        }

        private IEnumerator EndDashTrail()
        {
            yield return new WaitForSeconds(0.3f);
            trailRenderer.enabled = false;
        }

        private void Character_OnStatsUpdated(object sender, EventArgs e)
        {
            UpdatePickupRadius();
            UpdatePlayerSpeed();
            UpdatePlayerHealth();
        }

        private void UpdatePlayerHealth()
        {
            var newHealthPercentage = 1 + GetStatChange(StatType.Health);
            _health.MaximumHealth = _health.MaximumHealth * newHealthPercentage;
            _health.SetHealth(_health.CurrentHealth * newHealthPercentage);
        }

        private void UpdatePlayerSpeed()
        {
            var speedIncrease = GetStatValue(StatType.SpeedPercentage);
            var newSpeed = (1 + speedIncrease) * _baseSpeed;
            newSpeed = Mathf.Clamp(newSpeed, _minSpeed, _maxSpeed);
            _characterMovement.WalkSpeed = newSpeed;
            _characterMovement.ResetSpeed();
        }

        private void UpdatePickupRadius()
        {
            var pickupRadiusIncrease = GetStatValue(StatType.PickupRadiusPercentage);
            _pickupRadius.UpdatePickupRadius(pickupRadiusIncrease);
        }

        private void Update()
        {
            HandlePlayerAnimations();
            HandleRotation();
            StopShaking();
        }

        private void StopShaking()
        {
            if (_shakeTimer > 0f)
            {
                _shakeTimer -= Time.deltaTime;
                if (_shakeTimer <= 0f)
                {
                    CinemachineBasicMultiChannelPerlin perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                    perlin.m_AmplitudeGain = 0f;
                }
            }
        }

        private void HandlePlayerAnimations()
        {
            if (_handlePlayerInput == false) return;
            if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f)
            {
                _animator.SetBool("IsRunning", true);
            }
            else
            {
                _animator.SetBool("IsRunning", false);
            }
        }

        private void HandleRotation()
        {
            if (_handlePlayerInput == false) return;
            if (Input.GetAxisRaw("Horizontal") > 0f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (Input.GetAxisRaw("Horizontal") < 0f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }

        public override void CharacterDied()
        {
            _animator.SetBool("IsDead", true);
            _animator.SetTrigger("DoDying");
            //_dyingAudio.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(_dyingAudio);
            base.CharacterDied();
        }

        public override void CharacterHit()
        {
            int random = Random.Range(0, _hurtAudio.Length);
            //_hurtAudio[random].Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(_hurtAudio[random]);
            _animator.SetTrigger("DoHit"); 
            _playerHurtImage.Flash(0.5f, 0.15f, 0.25f, Color.red);
            ShakeCamera(1, 0.5f);
            StartCoroutine(DoTempInvulnerable(0.9f));
        }

        private void ShakeCamera(float intensity, float time)
        {
            CinemachineBasicMultiChannelPerlin perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = intensity;
            _shakeTimer = time;
        }

        private IEnumerator DoTempInvulnerable(float duration)
        {
            _health.Invulnerable = true;
            yield return new WaitForSeconds(duration);
            _health.Invulnerable = false;
        }

        internal void Attack(Weapon weapon)
        {
            int rand = Random.Range(0, 2);
            if (weapon != null)
            {
                if (rand == 0) _animator.SetTrigger("DoAttack2");
                if (rand == 1) _animator.SetTrigger("DoAttack2");
                if (rand == 2) _animator.SetTrigger("DoAttack2");
            }    
            
        }

        internal void Heal(float healthValue)
        {
            //_healedAudio.Play();
            AudioController.instance.PlayAudioSourceAsSfxClip(_healedAudio);
            _health.ReceiveHealth(healthValue, gameObject);
        }
    }
}
