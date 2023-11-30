using BackpackSurvivors.Backpack;
using BackpackSurvivors.MainGame;
using BackpackSurvivors.Shared;
using BackpackSurvivors.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Combat
{
    public class CombatController : MonoBehaviour
    {

        [SerializeField] GameObject _weaponPrefab;
        private List<Weapon> _weapons = new List<Weapon>();
        private bool _canAct = false;
        private Player _player;
        private PlayerDeathUI _playerDeathUI;

        public void SetCanAct(bool canAct)
        {
            _canAct = canAct;
        }

        public void InitLevel()
        {
            _player.InitHealth();
        }

        public void ReloadStats()
        {
            RefreshWeaponsFromBackpack();
            BackPackController.instance.CalculatePlayerPassivesFromBackpack();
            StatsController.instance.ApplyStatsToCharacter(_player, CharacterType.Player);
        }

        public bool IsFirstWeaponOfType(Weapon weapon)
        { 
            var firstWeaponOfType = _weapons.FirstOrDefault(w=> w.name == weapon.name);
            if(firstWeaponOfType == null) return false;

            var result = firstWeaponOfType.UniqueWeaponKey == weapon.UniqueWeaponKey;
            return result;
        }

        private void Start()
        {
            _player = FindObjectOfType<Player>();
            _playerDeathUI = FindObjectOfType<PlayerDeathUI>(true);
            RegisterPlayerDeath();
        }

        private void RegisterPlayerDeath()
        {
            _player.OnCharacterDied += Player_OnCharacterDeath;
        }

        private void Player_OnCharacterDeath(object sender, EventArgs e)
        {
            _canAct = false;
            _player.ClearWeapons();
            StartCoroutine(PlayerDeathCountdown());
        }

        private IEnumerator PlayerDeathCountdown()
        {
            _playerDeathUI.ShowPlayerDeathUI();
            var secondsToCountDown = 8;
            for (int i = secondsToCountDown; i > 0; i--)
            {
                _playerDeathUI.UpdateCountdownText(i.ToString());
                yield return new WaitForSeconds(1);
            }

            SceneManager.LoadScene(Constants.Scenes.MountainScalingScene);
        }

        private void Update()
        {
            if (_canAct == false) return;

            DecreaseCooldown();
            ActivateWeaponsOnZeroCooldown();
        }

        private void DecreaseCooldown()
        {
            var cooldownDecrease = Time.deltaTime;
            foreach (var weapon in _weapons)
            {
                weapon.DecreaseCooldown(cooldownDecrease);

                OnWeaponCooldownUpdate(this, new WeaponCooldownUpdateEventArgs(weapon));
            }
        }

        private void ActivateWeaponsOnZeroCooldown()
        {
            foreach (var weapon in _weapons)
            {
                if (weapon.CanAttack())
                {
                    weapon.Attack();

                    OnWeaponReady(this, new WeaponReadyEventArgs(weapon));
                }
            }
        }

        private Player GetPlayer()
        {
            if (_player == null) _player = FindObjectOfType<Player>();
            return _player;
        }

        private void RefreshWeaponsFromBackpack() 
        {
            foreach (var weapon in _weapons)
            {
                weapon.OnWeaponAttack -= Weapon_OnWeaponAttack;
            }

            _weapons.Clear();
            GetPlayer().ClearWeapons();

            if (OnWeaponsReset != null)
            {
                OnWeaponsReset(this, new WeaponsResetEventArgs());
            }            

            var weaponsInBackpack = BackpackController_GetWeapons();

            int prevId = -1;
            float currentDelay = 0f;
            float maxDelayIncrementer = 0.5f;

            foreach (var weaponFromBackpack in weaponsInBackpack.OrderBy(x => x.GetBackpackItem().Id))
            {
                var weaponGameobject = Instantiate(_weaponPrefab, _player.WeaponsParent.transform, false);
                Weapon weapon = weaponGameobject.GetComponent<Weapon>();
                var backpackItemSO = weaponFromBackpack.GetBackpackItem();


                if (prevId == backpackItemSO.Id) //we have another weapon like this
                {
                    int totalWeaponsOfThisType = weaponsInBackpack.Count(x => x.GetBackpackItem().Id == backpackItemSO.Id);
                    float delay = maxDelayIncrementer / totalWeaponsOfThisType;
                    currentDelay += delay;
                }
                else
                {
                    currentDelay = 0f;
                    prevId = backpackItemSO.Id; 
                }

                weapon.Init(backpackItemSO, _player, backpackItemSO.AttackType, currentDelay);
                _weapons.Add(weapon);

                weapon.OnWeaponAttack += Weapon_OnWeaponAttack;

                if (OnWeaponRegister != null)
                {
                    OnWeaponRegister(this, new WeaponRegisterEventArgs(weapon));
                }
            }
        }

        private void Weapon_OnWeaponAttack(object sender, WeaponAttackEventArgs e)
        {
            _player.Attack(e.Weapon);
        }

        private List<BackpackItem> BackpackController_GetWeapons()
        {
            return BackPackController.instance.GetAllWeapons();
        }

        public delegate void WeaponRegisterHandler(object sender, WeaponRegisterEventArgs e);
        public event WeaponRegisterHandler OnWeaponRegister;

        public delegate void WeaponReadyHandler(object sender, WeaponReadyEventArgs e);
        public event WeaponReadyHandler OnWeaponReady;

        public delegate void WeaponCooldownUpdateHandler(object sender, WeaponCooldownUpdateEventArgs e);
        public event WeaponCooldownUpdateHandler OnWeaponCooldownUpdate;

        public delegate void WeaponsResetHandler(object sender, WeaponsResetEventArgs e);
        public event WeaponsResetHandler OnWeaponsReset;
    }
}
